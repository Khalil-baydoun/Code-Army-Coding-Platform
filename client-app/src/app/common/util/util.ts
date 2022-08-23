import { pieData, verdicts } from "./commanData";

export const combineDateAndTime = (date: Date, time: Date) => {
  const timeString = time.getHours() + ":" + time.getMinutes() + ":00";
  const year = date.getFullYear();
  const month = date.getMonth() + 1;
  const day = date.getDate();
  const dateString = `${year}-${month}-${day}`;
  return new Date(dateString + " " + timeString);
};


export const getPieData = (verdictsCnt : Map<number, number>) => {
  let data: pieData[] = [];
  verdictsCnt.forEach((verdictCnt: number, verdictInd: number) => {
    let toadd: pieData = {
      title: verdicts[+verdictInd].summary,
      value: verdictCnt,
      color: verdicts[+verdictInd].color,
    };

    data.push(toadd);
  });
  return data;
}

export const getLabels = (data: pieData[]) => {
  let labels : string[] =[];
  data.forEach( (pie) => {
    if(pie.title){
      labels.push(""+pie.title);
    }
  });

  return labels;
}

