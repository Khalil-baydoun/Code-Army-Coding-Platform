import { observer } from "mobx-react-lite";
import React, { Fragment, useContext } from "react";
import { CSVLink } from "react-csv";
import { PieChart } from "react-minimal-pie-chart";
import { Grid, GridColumn, GridRow, Icon, Table } from "semantic-ui-react";
import {
  COLORS,
  pieData,
  verdicts,
  verdictsSummary,
} from "../../../app/common/util/commanData";
import { IUserStatistics } from "../../../app/models/courseProblemSet";
import { RootStoreContext } from "../../../app/stores/rootStore";

export function getData(courseStatistics: IUserStatistics[]) {
  let data: pieData[] = [];

  let verdictsCnt = new Map();
  courseStatistics.forEach((element) => {
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

function getExportData(courseStatistics: IUserStatistics[]) {
  let data: any = [];
  courseStatistics.forEach((element) => {
    data.push([
      element.UserEmail,
      element.NumberOfProblemsAttempted,
      element.NumberOfSolvedProblems,
      element.NumberOfSubmissions,
      element.NumberOfAcceptedSubmissions,
    ]);
  });
  return data;
}

const headers = [
  "User Email",
  "Problems Attempted",
  "Problems Solved",
  "Submissions",
  "Accepted Submissions",
];

const CourseStatistics: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { course, courseStatistics } = rootStore.courseProblemSetStore;
  if (!course || !courseStatistics) {
    return <h1>Not in a course!</h1>;
  }

  const renderPieChart = () => {
    let data = getData(courseStatistics);
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
    <div>
      <Grid>
        <GridRow>
          <GridColumn width={4}>{renderPieChart()}</GridColumn>
          <GridColumn width={12}>
            <Table celled selectable>
              <Table.Header>
                <Table.Row>
                  <Table.HeaderCell
                    colSpan="5"
                    textAlign="center"
                    style={{ fontSize: "20px" }}
                  >
                    Users Statistics List
                    <br />
                    <div
                      style={{
                        fontSize: "14px",
                        float: "right",
                        verticalAlign: "bottom",
                      }}
                    >
                      <CSVLink
                        data={getExportData(courseStatistics)}
                        filename={course.Name + " User Statistics.csv"}
                        headers={headers}
                      >
                        Export <Icon name="download" size="small"></Icon>
                      </CSVLink>
                    </div>
                  </Table.HeaderCell>
                </Table.Row>
                <Table.Row>
                  <Table.HeaderCell>UserEmail</Table.HeaderCell>
                  <Table.HeaderCell>Problems Attempted</Table.HeaderCell>
                  <Table.HeaderCell>Problems Solved </Table.HeaderCell>
                  <Table.HeaderCell>Submissions</Table.HeaderCell>
                  <Table.HeaderCell>Accepted Submissions</Table.HeaderCell>
                </Table.Row>
              </Table.Header>
              <Table.Body>
                {courseStatistics.map((summary: IUserStatistics) => (
                  <Table.Row>
                    <Table.Cell>
                      <p>{summary.UserEmail}</p>
                    </Table.Cell>
                    <Table.Cell>
                      <p>{summary.NumberOfProblemsAttempted}</p>
                    </Table.Cell>
                    <Table.Cell>
                      <p>{summary.NumberOfSolvedProblems}</p>
                    </Table.Cell>
                    <Table.Cell>
                      <p>{summary.NumberOfSubmissions}</p>
                    </Table.Cell>
                    <Table.Cell>
                      <p>{summary.NumberOfAcceptedSubmissions}</p>
                    </Table.Cell>
                  </Table.Row>
                ))}
              </Table.Body>
            </Table>
          </GridColumn>
        </GridRow>
      </Grid>
    </div>
  );
};

export default observer(CourseStatistics);
