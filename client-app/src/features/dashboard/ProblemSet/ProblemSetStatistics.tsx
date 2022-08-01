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
  COLORS,
  pieData,
  verdicts,
  verdictsSummary,
} from "../../../app/common/util/commanData";
import {
  IProblemSetStatistics,
  IProblemStatistics,
} from "../../../app/models/courseProblemSet";
import { RootStoreContext } from "../../../app/stores/rootStore";

export function getData(problemSetStatistics: IProblemSetStatistics) {
  let data: pieData[] = [];
  let verdictsCnt = new Map();
  problemSetStatistics.ProblemsStatistics.forEach((element) => {
    element.VerdictCounts.forEach((verdict) => {
      if (verdictsCnt.has(verdict.Key)) {
        verdictsCnt.set(
          verdict.Key,
          verdict.Value + parseInt(verdictsCnt.get(verdict.Key))
        );
      } else {
        verdictsCnt.set(verdict.Key, verdict.Value);
      }
    });
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

const ProblemSetStatistics: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    problemSetStatistics,
    problemSet,
    courseGroups,
    problemSetStatisticsRegistry,
  } = rootStore.courseProblemSetStore;
  const { problemRegistry } = rootStore.problemStore;

  if (!problemSet || !problemSetStatistics) {
    return <h1>Not in a problemSet!</h1>;
  }

  // console.log(getData({ ...problemSetStatisticsRegistry.get("1") }));
  // console.log({ ...problemSetStatisticsRegistry });

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
              label={({ dataIndex, dataEntry }) => verdictsSummary[dataIndex]}
              data={getData(problemSetStatistics)}
            />
          </GridColumn>
          <GridColumn width={12}>
            <Table celled selectable>
              <Table.Header>
                <Table.Row>
                  <Table.HeaderCell
                    colSpan="4"
                    textAlign="center"
                    style={{ fontSize: "20px" }}
                  >
                    Problems Statistics List
                    <br />
                    <div
                      style={{
                        fontSize: "14px",
                        float: "right",
                        verticalAlign: "bottom",
                      }}
                    ></div>
                  </Table.HeaderCell>
                </Table.Row>
                <Table.Row>
                  <Table.HeaderCell>ProblemId</Table.HeaderCell>
                  <Table.HeaderCell>ProblemName</Table.HeaderCell>
                  <Table.HeaderCell>Times Attempted</Table.HeaderCell>
                  <Table.HeaderCell>Times Solved </Table.HeaderCell>
                </Table.Row>
              </Table.Header>
              <Table.Body>
                {problemSetStatistics.ProblemsStatistics.map(
                  (summary: IProblemStatistics) => (
                    <Table.Row key={summary.ProblemId}>
                      <Table.Cell>
                        <p>{summary.ProblemId}</p>
                      </Table.Cell>

                      <Table.Cell>
                        <p>
                          {
                            problemRegistry.get(String(+summary.ProblemId))
                              .Title
                          }
                        </p>
                      </Table.Cell>
                      <Table.Cell>
                        <p>{summary.NumberOfTimesAttempted}</p>
                      </Table.Cell>
                      <Table.Cell>
                        <p>{summary.NumberOfTimesSolved}</p>
                      </Table.Cell>
                    </Table.Row>
                  )
                )}
              </Table.Body>
            </Table>
          </GridColumn>
        </GridRow>
        <Divider />
      </Grid>
      {courseGroups?.map((x) => (
        <Grid textAlign="center" key={x.Id}>
          {console.log(courseGroups)}
          <Header as="h2" style={{ marginTop: "20px" }}>
            {x.Name} Statistics
          </Header>
          <GridRow>
            <GridColumn width={4}>
              {console.log(x.Id.toString())}
              {console.log(problemSetStatisticsRegistry.get(x.Id.toString()))}
              {problemSetStatisticsRegistry.get(x.Id.toString()) && (
                <PieChart
                  style={{ fontSize: "7px" }}
                  label={({ dataIndex, dataEntry }) =>
                    verdictsSummary[dataIndex]
                  }
                  data={getData(problemSetStatisticsRegistry.get(x.Id.toString())!)}
                />
              )}
            </GridColumn>
            <GridColumn width={12}>
              <Table celled selectable>
                <Table.Header>
                  <Table.Row>
                    <Table.HeaderCell
                      colSpan="4"
                      textAlign="center"
                      style={{ fontSize: "20px" }}
                    >
                      Problems Statistics List
                      <br />
                      <div
                        style={{
                          fontSize: "14px",
                          float: "right",
                          verticalAlign: "bottom",
                        }}
                      ></div>
                    </Table.HeaderCell>
                  </Table.Row>
                  <Table.Row>
                    <Table.HeaderCell>ProblemId</Table.HeaderCell>
                    <Table.HeaderCell>ProblemName</Table.HeaderCell>
                    <Table.HeaderCell>Times Attempted</Table.HeaderCell>
                    <Table.HeaderCell>Times Solved </Table.HeaderCell>
                  </Table.Row>
                </Table.Header>
                <Table.Body>
                  {problemSetStatisticsRegistry.get(x.Id.toString()) &&
                    problemSetStatisticsRegistry
                      .get(x.Id.toString())!
                      .ProblemsStatistics.map((summary: IProblemStatistics) => (
                        <Table.Row key={summary.ProblemId}>
                          <Table.Cell>
                            <p>{summary.ProblemId}</p>
                          </Table.Cell>

                          <Table.Cell>
                            <p>
                              {
                                problemRegistry.get(String(+summary.ProblemId))
                                  .Title
                              }
                            </p>
                          </Table.Cell>
                          <Table.Cell>
                            <p>{summary.NumberOfTimesAttempted}</p>
                          </Table.Cell>
                          <Table.Cell>
                            <p>{summary.NumberOfTimesSolved}</p>
                          </Table.Cell>
                        </Table.Row>
                      ))}
                </Table.Body>
              </Table>
            </GridColumn>
          </GridRow>
          {/* <GridRow></GridRow> */}
          <Divider />
        </Grid>
      ))}
    </div>
  );
};

export default observer(ProblemSetStatistics);
