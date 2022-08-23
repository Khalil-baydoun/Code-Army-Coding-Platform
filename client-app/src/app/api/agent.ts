import axios, { AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { history } from "../..";
import {
  IAddUsersToCourseRequest,
  ICourse,
  ICourseStatistics,
  ICourseSummary,
  IProblemSet,
  IProblemSetStatistics,
  IProblemSetSummary,
  IUserStatistics,
  IUserSubmissions,
} from "../models/courseProblemSet";
import { IGetSolRequest } from "../models/GetSolRequest";
import { IProblem } from "../models/problem";
import { ISubmissionRequest } from "../models/submissionRequest";
import { IUser, IUserFormValues, IUserSignUpValues } from "../models/user";
axios.defaults.baseURL = "https://localhost:44381/api";

axios.interceptors.request.use(
  (config) => {
    const token = window.localStorage.getItem("jwt");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axios.interceptors.response.use(undefined, (error) => {
  if (error.message === "Network Error" && !error.response) {
    toast.error("Network error!");
  }
  const { status, data, config } = error.response;
  if (
    status === 400 &&
    config.method === "get" &&
    data.errors.hasOwnProperty("id")
  ) {
    console.log(error);
    history.push("/notfound");
  } else if (status === 400) {
  }
  if (status === 401) {
    window.localStorage.removeItem("jwt");
    history.push("/");
  }
  if (status === 403) {
    history.push("/forbidden");
  }
  if (status === 500) {
    toast.error("Server error!");
  }
  throw error.response;
});

const responseBody = (response: AxiosResponse) => response.data;

const sleep = (ms: number) => (response: AxiosResponse) =>
  new Promise<AxiosResponse>((resolve) =>
    setTimeout(() => resolve(response), ms)
  );

const requests = {
  get: (url: string) => axios.get(url).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
  del: (url: string) => axios.delete(url).then(responseBody),
  getWithParams: (url: string, parameters: {}) =>
    axios.get(url, { params: parameters }).then(responseBody),
};

const Problems = {
  list: (): Promise<IProblem[]> => requests.get("/problem"),
  openProblem: (id: string) => requests.get(`/problem/${id}`),
  loadReport: (submissionId: string) =>
    requests.get(`/submission/${submissionId}`),
  submitProblem: (submissionRequest: ISubmissionRequest) =>
    requests.post(`/submission`, submissionRequest),
  createProblem: (problem: IProblem) => requests.post("/problem", problem),
  updateProblem: (problem: IProblem) =>
    requests.put(`/problem/${problem.Id}`, problem),
  deleteProblem: (id: string) => requests.del(`/problem/${id}`),
  uploadSolution: (SolFormData: FormData) =>
    requests.post("/problem/solution", SolFormData),
  loadSolution: (getSolReq: IGetSolRequest) =>
    requests.getWithParams(`/problem/solution/${getSolReq.ProblemId}`, {
      ProgLang: getSolReq.ProgLanguage,
    }),
};

const Tests = {
  uploadTests: (testFormData: FormData) =>
    requests.post("/test/uploadTests", testFormData),
};

const Users = {
  getUserStatistics: (): Promise<IUserStatistics> =>
    requests.get(`/statistics/user`),
  current: (): Promise<IUser> => requests.get(`/user`),
  login: (user: IUserFormValues): Promise<IUser> =>
    requests.post(`/authentication/login`, user),
  signUp: (user: IUserSignUpValues): Promise<IUser> =>
    requests.post(`/user`, user),
  getSubmissions: (offset: number, limit: number): Promise<IUserSubmissions> =>
    requests.get(`/submission?offset=${offset}&limit=${limit}`),
};

const Courses = {
  createCourse: (course: ICourse) => requests.post("/course", course),
  getCourseStatistics: (id: string): Promise<ICourseStatistics> =>
    requests.get(`/statistics/course/${id}`),
  addUsersToCourse: (addUserReq: IAddUsersToCourseRequest) =>
    requests.post("/course/addusers", addUserReq),
  updateCourse: (course: ICourse) =>
    requests.put(`/course/${course.Id}`, course),
  getCourseSimple: (id: string): Promise<ICourseSummary> =>
    requests.get(`/course/${id}`),
  getCourseDetailed: (id: string): Promise<ICourse> =>
    requests.get(`/course/${id}`),
  deleteCourse: (id: string) => requests.del(`course/${id}`),
};

const ProblemSets = {
  createProblemSet: (problemSet: IProblemSet) =>
    requests.post("/problemset", problemSet),
  updateProblemSet: (problemSet: IProblemSet) =>
    requests.put(`/problemset/${problemSet.Id}`, problemSet),
  getPsSimple: (id: string): Promise<IProblemSetSummary> =>
    requests.get(`/problemset/${id}`),
  getPsDetailed: (id: string): Promise<IProblemSet> =>
    requests.get(`/problemset/${id}`),
  deleteProblemSet: (id: string) => requests.del(`/problemset/${id}`),
};

export default {
  Problems,
  Tests,
  Users,
  Courses,
  ProblemSets,
};
