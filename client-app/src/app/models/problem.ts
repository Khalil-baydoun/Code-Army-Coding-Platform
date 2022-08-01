export interface IProblem {
  Id: string;
  Title: string;
  AuthorEmail: string;
  Difficulty: string;
  GeneralDescription: string;
  IDescription: string;
  ODescription: string;
  TimeLimitInMilliseconds: Number;
  MemoryLimitInKiloBytes: Number;
  SampleInput: string;
  SampleOutput: string;
  Tags: string[];
  Hints: string[];
  Comments: IComment[];
  ProblemSetId: string;
}

export interface IComment {
  Id: string;
  CreatedAt: Date;
  Body: string;
  AuthorEmail: string;
  AuthorName: string;
}
export interface IProblemFormValues extends Partial<IProblem> {}

export class ProblemFormValues {
  Id: string = "";
  Title: string = "";
  Difficulty: string = "";
  GeneralDescription: string = "";
  IDescription: string = "";
  ODescription: string = "";
  Tags: string[] = [];
  Hints: string[] = [];
  SampleInput: string = "";
  ProblemSetId: string = "";
  SampleOutput: string = "";
  TimeLimitInMilliseconds: Number = 0;
  MemoryLimitInKiloBytes: Number = 0;
  AuthorEmail: string = "";
  TimeFactor: Number = 2;
  MemoryFactor: Number = 10;
  constructor(init?: Partial<ProblemFormValues>) {
    Object.assign(this, init);
  }
}
