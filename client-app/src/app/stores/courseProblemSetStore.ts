import { action, observable, runInAction } from "mobx";
import { toast } from "react-toastify";
import { history } from "../..";
import agent from "../api/agent";
import {
  ICourse,
  IProblemSet,
  IProblemSetStatistics,
  ICourseStatistics,
  IAddUsersToCourseRequest,
} from "../models/courseProblemSet";
import { RootStore } from "./rootStore";

export default class CourseProblemSetStore {
  rootStore: RootStore;
  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;
  }
  @observable submitting = false;
  @observable course: ICourse | null = null;
  @observable courseStatistics: ICourseStatistics | null = null;
  @observable problemSetStatistics: IProblemSetStatistics | null = null;
  @observable problemSet: IProblemSet | null | undefined = null;
  @observable loadingInitial = false;
  @observable problemSetRegistry = new Map();
  @observable problemSetStatisticsRegistry = new Map();
  @observable target = "";

  @action getCourse = async (courseId: string) => {
    if (this.course && courseId == this.course.Id) {
      return;
    }
    this.loadingInitial = true;
    try {
      const course = await agent.Courses.getCourseDetailed(courseId);
      runInAction(() => {
        this.course = course;
        this.loadingInitial = false;
      });
      console.log(course);
      runInAction("load problemSets", () => {
        course.ProblemSets.forEach((problemSet) => {
          if (problemSet.DueDate != null) {
            problemSet.DueDate = new Date(problemSet.DueDate);
          }

          this.problemSetRegistry.set(String(+problemSet.Id), problemSet);
        });
      });
    } catch (error) {
      runInAction(() => {
        this.loadingInitial = false;
      });
      console.log(error);
    }
  };

  @action getCourseStatistics = async (courseId: string) => {
    if(this.courseStatistics && this.course?.Id == this.courseStatistics.CourseId){
      return;
    }
    this.loadingInitial = true;
    try {
      const course = await agent.Courses.getCourseStatistics(courseId);
      runInAction(() => {
        this.courseStatistics = course;
        this.loadingInitial = false;
      });
    } catch (error) {
      runInAction(() => {
        this.loadingInitial = false;
      });
      console.log(error);
    }
  };

  @action getProblemSetStatistics = async (problemSetId: string, courseId: string) => {
    if (this.problemSetStatistics && this.problemSetStatistics.ProblemSetId == parseInt(problemSetId)) {
      return;
    }

    this.loadingInitial = true;
    await this.getCourseStatistics(courseId);
    try {
      runInAction(() => {
        this.courseStatistics?.ProblemSetStatistics?.forEach((problemSet) => {
          if(problemSet.ProblemSetId == parseInt(problemSetId)){
            this.problemSetStatistics =problemSet;
          }
        });
        console.log(this.problemSetStatistics?.UserStatistics);

        this.loadingInitial = false;
      });
    } catch (error) {
      runInAction(() => {
        this.loadingInitial = false;
      });
      console.log(error);
    }
  };

  getProblemSet = (id: string) => {
    return this.problemSetRegistry.get(id);
  };

  didSolveProblem = (userEmail: string, problemId: string) => {
    let succ = false;
    console.log("statsssssssss",this.problemSetStatistics, userEmail, problemId)
    this.problemSetStatistics?.UserStatistics.forEach((element) => {
      if (
        element.UserEmail == userEmail &&
        element.ProblemIdsSolved.includes(parseInt(problemId))
      ) {
        succ = true;
      }
    });
    return succ;
  };

  @action loadProblemSet = async (courseId: string, problemSetId: string) => {
    if (this.course && this.problemSet && problemSetId == this.problemSet.Id) {
      return this.problemSet;
    }
    this.loadingInitial = true;
    await this.getCourse(courseId);
    let problemSet = this.getProblemSet(problemSetId);
    if (problemSet) {
      runInAction(() => {
        this.loadingInitial = false;
        this.problemSet = problemSet;
      });
      runInAction("load problems", () => {
        this.rootStore.problemStore.problemRegistry.clear();
        this.problemSet?.Problems.forEach((problem) => {
          this.rootStore.problemStore.problemRegistry.set(
            String(+problem.Id),
            problem
          );
        });
      });
      return problemSet;
    } else {
      runInAction(() => {
        this.loadingInitial = false;
      });
      history.push("/notfound");
    }
  };
  @action createProblemSet = async (problemSet: IProblemSet) => {
    this.submitting = true;
    try {
      var response = await agent.ProblemSets.createProblemSet(problemSet);
      problemSet.Id = response.ProblemSetId;
      runInAction("create problem set", () => {
        this.problemSetRegistry.set(String(+problemSet.Id), problemSet);
        this.submitting = false;
      });
      history.push(response.ProblemSetId + "/problems");
    } catch (error) {
      runInAction("create problem set error", () => {
        this.submitting = false;
      });
      toast.error("Problem submitting data");
    }
  };

  @action updateProblemSet = async (problemSet: IProblemSet) => {
    this.submitting = true;
    try {
      await agent.ProblemSets.updateProblemSet(problemSet);
      runInAction("edit problemSet", () => {
        if (this.problemSet) {
          this.problemSet.Description = problemSet.Description;
          this.problemSet.Name = problemSet.Name;
          this.problemSet.Prerequisites = problemSet.Prerequisites;
          if (this.problemSet.DueDate != null) {
            this.problemSet.DueDate = new Date(this.problemSet.DueDate);
          }
        }
        this.problemSetRegistry.set(String(+problemSet.Id), this.problemSet);
        this.submitting = false;
      });
    } catch (error) {
      runInAction("edit problem set error", () => {
        this.submitting = false;
      });
      console.log(error);
      toast.error("Problem submitting data");
    }
  };

  @action createCourse = async (course: ICourse) => {
    this.submitting = true;
    try {
      var response = await agent.Courses.createCourse(course);
      course.Id = response.CourseId;
      runInAction("create course", () => {
        this.rootStore.userStore.user?.Courses.push(course);
        this.submitting = false;
      });
      history.push(response.CourseId + "/problemset");
    } catch (error) {
      runInAction("create course error", () => {
        this.submitting = false;
      });
      toast.error("Problem submitting data");
    }
  };

  @action addUsersToCourse = async (userEmails: string[]) => {
    this.submitting = true;
    try {
      var request: IAddUsersToCourseRequest = {
        courseId: "" + this.course?.Id,
        userEmails: userEmails,
      };
      await agent.Courses.addUsersToCourse(request);
      runInAction("add users to course", () => {
        this.course?.UsersEmails.push.apply(this.course.UsersEmails, userEmails);
        this.submitting = false;
      });
      return true;
    } catch (error) {
      runInAction("add users to course error", () => {
        this.submitting = false;
      });
      return false;
    }
  };

  // @action addUsersToCourse = async (userEmails: FormData) => {
  //   this.submitting = true;
  //   try {
  //     await agent.Courses.addUsersToCourse(userEmails);
  //     var newCrs = await agent.Courses.getCourseSimple(this.course?.Id!);
  //     runInAction("add users to course", () => {
  //       if (this.course) {
  //         this.course.UsersEmails = newCrs.UsersEmails;
  //       }
  //       this.submitting = false;
  //     });
  //     return true;
  //   } catch (error) {
  //     runInAction("add users to course error", () => {
  //       this.submitting = false;
  //     });
  //     return false;
  //   }
  // };

  @action updateCourse = async (course: ICourse) => {
    this.submitting = true;
    try {
      await agent.Courses.updateCourse(course);
      runInAction("edit course", () => {
        if (this.course) {
          this.course.Description = course.Description;
          this.course.Name = course.Name;
        }
        if (this.rootStore.userStore.user?.Courses) {
          for (
            let i = 0;
            i < this.rootStore.userStore.user?.Courses.length!;
            i++
          ) {
            if (this.rootStore.userStore.user.Courses[i].Id == course.Id) {
              this.rootStore.userStore.user.Courses[i].Name = course.Name;
              this.rootStore.userStore.user.Courses[i].Description =
                course.Description;
            }
          }
        }
        this.submitting = false;
      });
    } catch (error) {
      runInAction("edit course error", () => {
        this.submitting = false;
      });
      console.log(error);
      toast.error("Problem submitting data");
    }
  };

  @action deleteCourse = async (id: string) => {
    this.submitting = true;
    this.target = id;
    try {
      await agent.Courses.deleteCourse(id);
      runInAction("delete course", () => {
        if (this.course && this.course.Id == id) {
          this.course = null;
        }
        if (this.rootStore.userStore.user?.Courses) {
          this.rootStore.userStore.user!.Courses = this.rootStore.userStore.user!.Courses.filter(
            (c) => c.Id != id
          );
        }
        this.submitting = false;
        this.target = "";
        history.push("/course");
      });
    } catch (error) {
      runInAction("delete course error", () => {
        this.submitting = false;
        this.target = "";
      });
      console.log(error);
      history.push("/course");
    }
  };

  @action deleteProblemSet = async (id: string, courseId: string) => {
    this.submitting = true;
    this.target = id;
    try {
      await agent.ProblemSets.deleteProblemSet(id);
      runInAction("delete problem set", () => {
        if (this.problemSet && this.problemSet.Id == id) {
          this.problemSet = null;
        }
        if (this.course?.Id == courseId) {
          this.course.ProblemSets! = this.course.ProblemSets.filter(
            (p) => p.Id != id
          );
        }
        this.problemSetRegistry.delete(String(+id));
        if (this.problemSetRegistry.size == 0) {
          this.problemSetRegistry = new Map();
        }
        this.submitting = false;
        this.target = "";
        history.push(`/course/${courseId}/problemset`);
      });
    } catch (error) {
      runInAction("delete problem set error", () => {
        this.submitting = false;
        this.target = "";
      });
      console.log(error);
      history.push(`/course/${courseId}/problemset`);
    }
  };

  @action cleanUpProblemSetStatisticsRegistry = () => {
    this.problemSetStatisticsRegistry = new Map();
  };
}
