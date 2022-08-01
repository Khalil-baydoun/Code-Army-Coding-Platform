import { ICourseSummary } from "./courseProblemSet";

export interface IUser {
  Email: string;
  FirstName: string;
  LastName: string;
  Token: string;
  Role: string;
  Courses: ICourseSummary[];
}

export interface IUserFormValues {
  Email: string;
  Password: string;
  FirstName: string;
  LastName: string;
}

export interface IUserSignUpValues {
  Email: string;
  FirstName: string;
  LastName: string;
  Role: number;
  CourseIds: ICourseSummary[];
  Password: string;
}
