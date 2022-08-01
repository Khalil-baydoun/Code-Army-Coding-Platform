import { configure } from "mobx";
import { createContext } from "react";
import CommonStore from "./commonStore";
import CourseProblemSetStore from "./courseProblemSetStore";
import ModalStore from "./modalStore";
import ProblemStore from "./problemStore";
import UserStore from "./userStore";

configure({ enforceActions: "always" });

export class RootStore {
  problemStore: ProblemStore;
  userStore: UserStore;
  commonStore: CommonStore;
  modalStore: ModalStore;
  courseProblemSetStore: CourseProblemSetStore;

  constructor() {
    this.problemStore = new ProblemStore(this);
    this.userStore = new UserStore(this);
    this.commonStore = new CommonStore(this);
    this.modalStore = new ModalStore(this);
    this.courseProblemSetStore = new CourseProblemSetStore(this);
  }
}

export const RootStoreContext = createContext(new RootStore());
