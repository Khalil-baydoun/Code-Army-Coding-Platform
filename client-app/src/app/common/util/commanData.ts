export interface Verdict {
  summary: string,
  name: string,
  color: string
};

export const verdicts: {[index:number]:Verdict } = {
  15: {
    name: "Accepted",
    summary: "ACC",
    color: "#0cdf59"
  },
  2:
  {
    name: "Wrong Answer",
    summary: "WA",
    color: "#ff2f3f"
  },
  11:
  {
    name: "Compilation Error",
    summary: "CE",
    color: "#FFBB28"
  },
  12:
  {
    name: "Runtime Error",
    summary: "RE",
    color: "#FF8042"
  },
  17:
  {
    name: "Memory Limit Exceeded",
    summary: "MLE",
    color: "#f776c6"
  },
  13:
  {
    name: "Time Limit Exceeded",
    summary: "TLE",
    color: "#0073ff"
  },
  1:
  {
    name: "In Queue",
    summary: "IQ",
    color: "#f86a2c"
  }
};

export function difficultyToString(difficulty: any) {
  let difficulties = ["EASY", "MEDIUM", "HARD"];
  return difficulties[difficulty];
}

export interface pieData {
  color: string;
  value: number;
  key?: string | number;
  title?: string | number;
  [key: string]: any;
}
