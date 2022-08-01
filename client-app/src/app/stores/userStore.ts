import { action, computed, observable, runInAction } from "mobx";
import { history } from "../..";
import agent from "../api/agent";
import { IUserStatistics, IUserSubmissions } from "../models/courseProblemSet";
import { IUser, IUserFormValues, IUserSignUpValues } from "../models/user";
import { RootStore } from "./rootStore";

export default class UserStore {
  rootStore: RootStore;
  submissionsPageSize: number = 20;

  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;
  }

  @observable user: IUser | null = null;
  @observable submissions: IUserSubmissions | null = null;
  @observable statistics: IUserStatistics | null = null;
  @observable loadingInitial = false;

  @computed get isLoggedIn() {
    return !!this.user;
  }

  @action login = async (values: IUserFormValues) => {
    try {
      const user = await agent.Users.login(values);
      runInAction(() => {
        this.user = user;
      });
      this.rootStore.commonStore.setToken(user.Token);
      this.rootStore.modalStore.closeModal();
      history.push("/course");
    } catch (error) {
      throw error;
    }
  };

  @action signup = async (values: IUserSignUpValues) => {
    try {
      values.CourseIds = [];
      values.Role = 0;

      await agent.Users.signUp(values);
      const { CourseIds, Role, ...loginValues } = values;

      const user = await agent.Users.login(loginValues);
      runInAction(() => {
        this.user = user;
      });
      this.rootStore.commonStore.setToken(user.Token);
      this.rootStore.modalStore.closeModal();
      history.push("/course");
    } catch (error) {
      throw error;
    }
  };

  @action getUserSubmissions = async (page: number) => {
    this.loadingInitial = true;
    try {
      const submissions = await agent.Users.getSubmissions(
        (page - 1) * this.submissionsPageSize,
        this.submissionsPageSize
      );
      runInAction(() => {
        this.submissions = submissions;
        this.loadingInitial = false;
      });
    } catch (error) {
      runInAction(() => {
        this.loadingInitial = false;
      });
      console.log(error);
      throw error;
    }
  };

  @action getUserStatistics = async () => {
    this.loadingInitial = true;
    try {
      const statistics = await agent.Users.getUserStatistics();
      runInAction(() => {
        this.statistics = statistics;
        this.loadingInitial = false;
      });
    } catch (error) {
      runInAction(() => {
        this.loadingInitial = false;
      });
      console.log(error);
      throw error;
    }
  };

  @action getUser = async () => {
    try {
      const user = await agent.Users.current();
      runInAction(() => {
        this.user = user;
      });
    } catch (error) {
      this.rootStore.commonStore.setToken(null);
    }
  };

  @action logout = () => {
    this.rootStore.commonStore.setToken(null);
    this.user = null;
    history.push("/");
  };

  isAuthorizedToEditCourse = () => {
    return (
      this.hasRole("Admin") ||
      (this.hasRole("Instructor") &&
        this.rootStore.courseProblemSetStore.course &&
        this.rootStore.courseProblemSetStore.course &&
        this.rootStore.courseProblemSetStore.course.AuthorEmail ===
          this.user?.Email)
    );
  };
  hasRole = (role: string) => {
    let difficulties = ["", "User", "Instructor", "Admin"];
    return (
      this.user && this.user.Role && difficulties[+this.user?.Role!] === role
    );
  };
}
