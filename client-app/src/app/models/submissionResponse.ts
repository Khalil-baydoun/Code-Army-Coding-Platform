export interface ISubmissionReport {
  Id: number;
  WaReport: IWrongAnswerReport;
}

export interface IWrongAnswerReport {
  ActualOutput: string;
  Input: string;
  ExpectedOutput: string;
}
