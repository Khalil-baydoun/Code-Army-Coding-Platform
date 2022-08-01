export interface ISubmissionReport {
  StaticCodeAnalysis: string[];
  Id: number;
  WaReport: IWrongAnswerReport;
}

export interface IWrongAnswerReport {
  ActualOutput: string;
  Input: string;
  ExpectedOutput: string;
}
