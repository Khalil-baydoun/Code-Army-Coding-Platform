export const verdicts = [
  "Accepted",
  "Wrong Answer",
  "Compilation Error",
  "Runtime Error",
  "Memory Limit Exceeded",
  "Time Limit Exceeded",
  "In Queue",
];

export function difficultyToString(difficulty: any) {
  let difficulties = ["EASY", "MEDIUM", "HARD"];
  return difficulties[difficulty];
}

export const verdictsSummary = ["AC", "WA", "CE", "RE", "MLE", "TLE", "IQ"];

export const COLORS = [
  "#0cdf59",
  "#ff2f3f",
  "#FFBB28",
  "#FF8042",
  "#f776c6",
  "#0073ff",
  "#f86a2c",
];

export interface pieData {
  color: string;
  value: number;
  key?: string | number;
  title?: string | number;
  [key: string]: any;
}
