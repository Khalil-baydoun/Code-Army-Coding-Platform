import { observer } from "mobx-react-lite";
import React, { useContext, useEffect } from "react";
import { PieChart } from "react-minimal-pie-chart";
import {
  Divider,
  Grid,
  GridColumn,
  GridRow,
  Header,
  Segment,
  Table,
} from "semantic-ui-react";
import {
  pieData,
  verdicts,
} from "../../../app/common/util/commanData";
import { getPieData } from "../../../app/common/util/util";
import {
  IProblemSetStatistics,
  IProblemStatistics,
} from "../../../app/models/courseProblemSet";
import { RootStoreContext } from "../../../app/stores/rootStore";

export function getData(problemSetStatistics: IProblemSetStatistics) {
  let data: pieData[] = [];
  let verdictsCnt = new Map();
  // TODO
  return getPieData(verdictsCnt);
}

const ProblemSetStatistics: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    problemSetStatistics,
    problemSet,
    problemSetStatisticsRegistry,
  } = rootStore.courseProblemSetStore;
  const { problemRegistry } = rootStore.problemStore;

  if (!problemSet || !problemSetStatistics) {
    return <h1>Not in a problemSet!</h1>;
  }

  return (
    <div>
      <Grid textAlign="center">
        <Header as="h2" style={{ marginTop: "20px" }}>
          General Statistics
        </Header>
        <GridRow>
          <GridColumn width={4}>
            <PieChart
              style={{ fontSize: "7px" }}
              label={({ dataIndex, dataEntry }) => verdicts[dataIndex].summary}
              data={getData(problemSetStatistics)}
            />
          </GridColumn>
        </GridRow>
        <Divider />
      </Grid>
    </div>
  );
};

export default observer(ProblemSetStatistics);
