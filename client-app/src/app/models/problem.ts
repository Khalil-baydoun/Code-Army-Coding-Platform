export interface IProblem {
  Id: string;
  Title: string;
  AuthorEmail: string;
  Difficulty: string;
  GeneralDescription: string;
  InputDescription: string;
  OutputDescription: string;
  TimeLimitInMilliseconds: Number;
  MemoryLimitInKiloBytes: Number;
  SampleInput: string;
  SampleOutput: string;
  Tags: string[];
  Hints: string[];
}

export interface IProblemFormValues extends Partial<IProblem> {}

export class ProblemFormValues {
  Id: string = "";
  Title: string = "";
  Difficulty: string = "";
  GeneralDescription: string = "";
  InputDescription: string = "";
  OutputDescription: string = "";
  Tags: string[] = [];
  Hints: string[] = [];
  SampleInput: string = "";
  SampleOutput: string = "";
  TimeLimitInMilliseconds: Number = 0;
  MemoryLimitInKiloBytes: Number = 0;
  AuthorEmail: string = "";
  constructor(init?: Partial<ProblemFormValues>) {
    Object.assign(this, init);
  }
}
