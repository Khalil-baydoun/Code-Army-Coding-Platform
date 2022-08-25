import { IProblemSummary } from "./problemSummary";

export interface ICourseSummary {
  Id: string;
  Name: string;
  Description: string;
  UsersEmails: string[];
}

export interface IKeyValuePair {
  Key: number;
  Value: number;
}

export interface ICourseStatistics {
  CourseId : string;
  UserStatistics: IUserStatistics[];
  ProblemSetStatistics : IProblemSetStatistics [];
}

export interface IUserSubmissions {
  Submissions: ISubmission[];
  SubmissionsRemaining: boolean;
}

export interface IProblemSetStatistics {
  ProblemSetId: number;
  UserStatistics: IProblemSetUserStatistics[];
}

export interface ISubmission {
  Id: number;
  UserEmail: string;
  ProblemId: number;
  Verdict: number;
  SourceCode: string;
  SubmittedAt: Date;
  ProgrammingLanguage: number;
}

export interface IUserStatistics {
  UserEmail: string;
  NumberOfSubmissions: number;
  NumberOfAcceptedSubmissions: number;
  NumberOfProblemsAttempted: number;
  NumberOfSolvedProblems: number;
  VerdictCounts: IKeyValuePair[];
}

export interface IProblemStatistics {
  ProblemId: number;
  NumberOfTimesAttempted: number;
  NumberOfTimesSolved: number;
  VerdictCounts: IKeyValuePair[];
}

export interface IProblemSetUserStatistics {
  UserEmail: string;
  ProblemIdsSolved: number[];
}

export interface ICourse {
  Id: string;
  Name: string;
  Description: string;
  UsersEmails: string[];
  ProblemSets: IProblemSet[];
  AuthorEmail: string;
}

export interface IProblemSetSummary {
  Id: string;
  Name: string;
  CourseId: string;
  Prerequisites: string[];
}

export interface IProblemSet {
  Id: string;
  Name: string;
  Description: string;
  AuthorEmail: string;
  Problems: IProblemSummary[];
  Prerequisites: string[];
  CourseId: string;
  DueDate?: Date;
}

export interface IProblemSetFormValues extends Partial<IProblemSet> {}

export class ProblemSetFormValues {
  Id: string = "";
  Name: string = "";
  Description: string = "";
  AuthorEmail: string = "";
  Problems: IProblemSummary[] = [];
  CourseId: string = "";
  Prerequisites: string[] = [];
  DueDate?: Date | null = null;
  constructor(init?: Partial<ProblemSetFormValues>) {
    Object.assign(this, init);
  }
}

export interface ICourseFormValues extends Partial<ICourse> {}

export class CourseFormValues {
  Id: string = "";
  Name: string = "";
  Description: string = "";
  UsersEmails: string[] = [];
  ProblemSets: IProblemSet[] = [];
  AuthorEmail: string = "";
  constructor(init?: Partial<CourseFormValues>) {
    Object.assign(this, init);
  }
}

export interface IGroup {
  Name: string;
  Id: string;
}


export interface IAddUsersToCourseRequest {
  courseId: string,
  userEmails: string[]
}