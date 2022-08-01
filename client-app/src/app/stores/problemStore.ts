import { HubConnection, HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
import { action, computed, configure, observable, runInAction } from "mobx";
import { toast } from "react-toastify";
import { history } from "../..";
import agent from "../api/agent";
import { IGetSolRequest } from "../models/GetSolRequest";
import { IComment, IProblem } from "../models/problem";
import { ISubmissionRequest } from "../models/submissionRequest";
import { ISubmissionReport } from "../models/submissionResponse";
import { RootStore } from "./rootStore";

configure({ enforceActions: "always" });

export default class ProblemStore {
  rootStore: RootStore;
  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;
  }

  @observable problemRegistry = new Map();
  @observable loadingInitial = false;
  @observable submitting = false;
  @observable searchName: string | null = null;
  @observable searchDifficulty: number | null = null;
  @observable problem: IProblem | null = null;
  @observable report: ISubmissionReport | null = null;
  @observable.ref hubConnection: HubConnection | null = null;
  @observable target = "";
  @observable solution: string | null = null;

  @action createHubConnection = () => {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl("http://localhost:5000/chat", {
        accessTokenFactory: () => this.rootStore.commonStore.token!,
      })
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log(this.hubConnection!.state))
      .catch((error) => console.log("Error establishing conenction", error));

    this.hubConnection.on("ReceiveComment", (comment: IComment) => {
      runInAction(() => {
        this.problem!.Comments.push(comment);
      });
    });
  };

  @action stopHubConnection = () => {
    this.hubConnection!.stop();
  };

  @action addComment = async (values: any) => {
    values.problemId = this.problem!.Id;
    try {
      await this.hubConnection!.invoke(
        "SendComment",
        values.Body,
        values.problemId
      );
    } catch (error) {
      console.log(error);
    }
  };

  @computed get problemSummariesById() {
    let probs = Array.from(this.problemRegistry.values()).sort(
      (a, b) => a.Id - b.Id
    );
    if (this.searchName != null && this.searchName.length > 0) {
      probs = probs.filter(
        (x) => x.Id === this.searchName || x.Title === this.searchName
      );
    }

    if (this.searchDifficulty != null) {
      probs = probs.filter((x) => x.Difficulty + 1 === this.searchDifficulty);
    }

    return probs;
  }

  getProblem = (id: string) => {
    return this.problemRegistry.get(id);
  };

  @action loadProblem = async (
    id: string,
    courseId: string,
    problemSetId: string
  ) => {
    if (this.problem && this.problem.Id == id) return this.problem;
    this.loadingInitial = true;
    await this.rootStore.courseProblemSetStore.loadProblemSet(
      courseId,
      problemSetId
    );
    let problem = this.getProblem(id);
    if (problem) {
      runInAction("loading problem ...", () => {
        this.problem = problem;
        this.loadingInitial = false;
      });

      return problem;
    } else {
      runInAction(() => {
        this.loadingInitial = false;
      });
      history.push("/notfound");
    }
  };

  @action modifySearch = (request: any) => {
    this.searchName = request.Title;
    this.searchDifficulty = request.Difficulty;
  };

  @action resetSearch = (request: any) => {
    this.searchName = null;
    this.searchDifficulty = null;
  };

  @action createProblem = async (problem: IProblem) => {
    this.submitting = true;
    try {
      var response = await agent.Problems.createProblem(problem);
      problem.Id = response.ProblemId;
      runInAction("create problem", () => {
        this.problemRegistry.set(String(+problem.Id), problem);
        this.submitting = false;
      });
      history.push(`problems/${problem.Id}`);
    } catch (error) {
      runInAction("create problem error", () => {
        this.submitting = false;
      });
      console.log(error);
      toast.error("Problem submitting data");
    }
  };

  @action uploadTests = async (formData: FormData) => {
    this.submitting = true;
    try {
      await agent.Tests.uploadTests(formData);
      runInAction("uploading tests", () => {
        this.submitting = false;
      });
    } catch (error) {
      runInAction("uploading tests error", () => {
        this.submitting = false;
      });
      console.log(error);
      toast.error("Problem submitting data");
    }
  };

  @action uploadSolution = async (formData: FormData) => {
    this.submitting = true;
    try {
      await agent.Problems.uploadSolution(formData);
      runInAction("uploading problem solution", () => {
        this.submitting = false;
      });
    } catch (error) {
      runInAction("uploading problem solution", () => {
        this.submitting = false;
      });
      console.log(error);
      toast.error("Problem submitting data");
    }
  };

  @action loadSolution = async (ProblemId: string, ProgLanguage: Number) => {
    this.submitting = true;
    const req: IGetSolRequest = { ProblemId, ProgLanguage };
    try {
      var solution = await agent.Problems.loadSolution(req);
      runInAction("loading problem solution", () => {
        this.solution = solution;
        this.submitting = false;
      });
    } catch (error) {
      runInAction("loading problem solution", () => {
        this.submitting = false;
        this.solution = null;
      });
      console.log(error);
      // toast.error("Problem loading solution");
    }
  };

  @action editProblem = async (problem: IProblem) => {
    this.submitting = true;
    try {
      await agent.Problems.updateProblem(problem);
      runInAction("edit problem", () => {
        this.problemRegistry.set(String(+problem.Id), problem);
        this.problem = problem;
        this.submitting = false;
      });
    } catch (error) {
      runInAction("edit problem error", () => {
        this.submitting = false;
      });
      console.log(error);
      toast.error("Problem submitting data");
    }
  };

  @action LoadReport = async (submissionId: string) => {
    let report: ISubmissionReport | null = null;
    {
      console.log("subid", submissionId);
    }
    this.loadingInitial = true;
    try {
      report = await agent.Problems.loadReport(submissionId);
      runInAction("loading report....", () => {
        this.report = report;
        this.loadingInitial = false;
        console.log("report", report);
      });
    } catch (error) {
      runInAction("loading report error", () => {
        this.loadingInitial = false;
      });
      console.log(error);
      toast.error("Problem loading report");
    }
  };

  @action Submit = async (submission: ISubmissionRequest) => {
    let report: ISubmissionReport | null = null;
    this.loadingInitial = true;
    try {
      report = await agent.Problems.submitProblem(submission);
      runInAction("submitting....", () => {
        this.report = report;
        history.push("/submissions/page/1");
        this.loadingInitial = false;
      });
    } catch (error) {
      runInAction("submitting error", () => {
        this.loadingInitial = false;
      });
      console.log(error);
      toast.error("Problem submitting data");
    }
  };

  @action clearProblem = () => {
    this.problem = null;
  };

  @action loadProblems = async () => {
    this.loadingInitial = true;
    try {
      const problemSummaries = await agent.Problems.list();
      runInAction("load problems", () => {
        problemSummaries.forEach((problem) => {
          this.problemRegistry.set(+problem.Id, problem);
        });
        this.loadingInitial = false;
      });
    } catch {
      runInAction("load problems error", () => {
        this.loadingInitial = false;
      });
    }
  };

  @action deleteProblem = async (id: string) => {
    this.submitting = true;
    this.target = id;
    try {
      await agent.Problems.deleteProblem(id);
      runInAction("delete problem", () => {
        if (this.problem && this.problem.Id == id) {
          this.problem = null;
        }
        this.problemRegistry.delete(String(+id));
        if (this.problemRegistry.size == 0) {
          this.problemRegistry = new Map();
        }
        this.submitting = false;
        this.target = "";
      });
    } catch (error) {
      runInAction("delete problem error", () => {
        this.submitting = false;
        this.target = "";
      });
      console.log(error);
    }
  };
}
