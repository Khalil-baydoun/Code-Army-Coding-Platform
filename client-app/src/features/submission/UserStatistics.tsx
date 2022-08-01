import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useEffect } from "react";
import { PieChart } from "react-minimal-pie-chart";
import { Grid, GridColumn, GridRow, Header, Table } from "semantic-ui-react";
import {
  COLORS,
  pieData,
  verdicts,
  verdictsSummary,
} from "../../app/common/util/commanData";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { IUserStatistics } from "../../app/models/courseProblemSet";
import { RootStoreContext } from "../../app/stores/rootStore";

export function getData(userStats: IUserStatistics) {
  let data: pieData[] = [];
  let verdictsCnt = new Map();
  userStats.VerdictCounts.forEach((verdict) => {
    if (verdictsCnt.has(verdict.Key)) {
      verdictsCnt.set(
        verdict.Key,
        verdict.Value + parseInt(verdictsCnt.get(verdict.Key))
      );
    } else {
      verdictsCnt.set(verdict.Key, verdict.Value);
    }
  });
  for (let i = 0; i < verdicts.length; i++) {
    if (verdictsCnt.has(i)) {
      let toadd: pieData = {
        title: verdictsSummary[i],
        value: verdictsCnt.get(i),
        color: COLORS[i],
      };
      data.push(toadd);
    }
  }
  return data;
}

const UserStatistics: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    user,
    statistics,
    getUserStatistics,
    loadingInitial,
  } = rootStore.userStore;

  useEffect(() => {
    if (user) {
      getUserStatistics();
    }
  }, [getUserStatistics, user]);

  if (!user) {
    return <h1>Not logged in!</h1>;
  }
  if (loadingInitial == true) {
    return <LoadingComponent />;
  }
  if (!statistics) {
    return <h1>Not Logged in!</h1>;
  }

  const renderPieChart = () => {
    let data = getData(statistics);
    if (data.length == 0) {
      return <h3>No data for pie chart</h3>;
    } else {
      return (
        <Fragment>
          <h3 style={{ color: "rgb(52, 50, 82)" }}>Verdicts Pie Chart:</h3>
          <PieChart
            style={{ fontSize: "7px" }}
            label={({ dataIndex }) => verdictsSummary[dataIndex]}
            data={data}
          />
        </Fragment>
      );
    }
  };

  return (
    <Fragment>
      <Header
        as="h2"
        icon="chart line"
        style={{ color: "rgb(52, 50, 82)" }}
        content="My Statistics"
      />
      <hr style={{ marginBottom: "3em" }} />
      <Grid>
        <GridRow>
          <GridColumn width={4}>{renderPieChart()}</GridColumn>
          <GridColumn width={12}>
            <Table definition>
              <Table.Body>
                <Table.Row>
                  <Table.Cell
                    colSpan="2"
                    textAlign="center"
                    style={{ fontSize: "20px" }}
                  >
                    Statistics Table
                  </Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell width={2}>UserEmail</Table.Cell>
                  <Table.Cell>{statistics.UserEmail}</Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell>Problems Attempted</Table.Cell>
                  <Table.Cell>
                    {statistics.NumberOfProblemsAttempted}
                  </Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell>Problems Solved</Table.Cell>
                  <Table.Cell> {statistics.NumberOfSolvedProblems}</Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell>Submissions</Table.Cell>
                  <Table.Cell>{statistics.NumberOfSubmissions}</Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell>Accepted Submissions</Table.Cell>
                  <Table.Cell>
                    {statistics.NumberOfAcceptedSubmissions}
                  </Table.Cell>
                </Table.Row>
                <Table.Row>
                  <Table.Cell>Acceptance Percentage</Table.Cell>
                  <Table.Cell>
                    {statistics.NumberOfSubmissions > 0
                      ? Math.round(
                          (statistics.NumberOfAcceptedSubmissions /
                            statistics.NumberOfSubmissions) *
                            10000
                        ) / 100
                      : 0}
                    %
                  </Table.Cell>
                </Table.Row>
              </Table.Body>
            </Table>
          </GridColumn>
        </GridRow>
      </Grid>
    </Fragment>
  );
};

export default observer(UserStatistics);
