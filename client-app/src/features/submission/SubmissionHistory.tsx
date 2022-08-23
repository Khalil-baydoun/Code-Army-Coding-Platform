import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useEffect, useState } from "react";
import { RouteComponentProps } from "react-router-dom";
import { Header, Icon, Modal, Table } from "semantic-ui-react";
import { verdicts } from "../../app/common/util/commanData";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { ISubmission } from "../../app/models/courseProblemSet";
import { RootStoreContext } from "../../app/stores/rootStore";
import SubmissonReport from "../problem/SubmissonReport";

export function verdictToString(verdict: any) {
  if (verdict == 15) {
    return <p style={{ color: "#279641" }}>{verdicts[verdict].summary}</p>;
  } else if (verdict == 1)
    return <p style={{ color: "#f86a2c" }}>{verdicts[verdict].summary}</p>;
  else {
    return <p style={{ color: "#be1e2b" }}>{verdicts[verdict].summary}</p>;
  }
}

interface DetailParams {
  pageNumber: string;
}

const SubmissionHistory: React.FC<RouteComponentProps<DetailParams>> = ({
  match,
  history,
}) => {
  const rootStore = useContext(RootStoreContext);
  const [activeCode, setActiveCode] = useState(-1);
  const {
    user,
    submissions,
    getUserSubmissions,
    loadingInitial,
  } = rootStore.userStore;

  useEffect(() => {
    if (user) {
      getUserSubmissions(parseInt(match.params.pageNumber));
    }
  }, [getUserSubmissions, user, match.params.pageNumber]);

  if (!user) {
    return <h1>Not logged in!</h1>;
  }
  if (loadingInitial == true) {
    return <LoadingComponent />;
  }

  return (
    <Fragment>
      <Header
        as="h2"
        icon="send"
        style={{ color: "rgb(52, 50, 82)" }}
        content="My Submissions"
      />
      <hr style={{ marginBottom: "3em" }} />
      <Fragment>
        <Table celled selectable>
          <Table.Header>
            <Table.Row>
              <Table.HeaderCell
                colSpan="7"
                textAlign="center"
                style={{ fontSize: "20px" }}
              >
                Submission History
              </Table.HeaderCell>
            </Table.Row>
            <Table.Row>
              <Table.HeaderCell>#</Table.HeaderCell>
              <Table.HeaderCell>ProblemId</Table.HeaderCell>
              <Table.HeaderCell>Verdict</Table.HeaderCell>
              <Table.HeaderCell>Submitted At</Table.HeaderCell>
              <Table.HeaderCell>Language</Table.HeaderCell>
            </Table.Row>
          </Table.Header>
          <Table.Body>
            {submissions?.Submissions.map(
              (summary: ISubmission, index: number) => (
                <Table.Row key={index}>
                  <Table.Cell width="1" style={{ cursor: "pointer" }}>
                    <div onClick={() => setActiveCode(index)}>
                      <p> {summary.Id}</p>
                    </div>
                  </Table.Cell>
                  <Table.Cell width="1">
                    <p>{summary.ProblemId}</p>
                  </Table.Cell>
                  <Table.Cell width="3">
                    {verdictToString(summary.Verdict)}
                  </Table.Cell>
                  <Table.Cell width="4">
                    <p>
                      {format(summary.SubmittedAt!, "eeee do MMMM")} at{" "}
                      {format(summary.SubmittedAt!, "h:mm a")}
                    </p>
                  </Table.Cell>
                  <Table.Cell width="4">
                    <p>{summary.ProgrammingLanguage}</p>
                  </Table.Cell>
                </Table.Row>
              )
            )}
          </Table.Body>
          <Table.Footer>
            <Table.Row>
              <Table.HeaderCell colSpan="7">
                <div style={{ float: "right" }}>
                  {parseInt(match.params.pageNumber) > 1 && (
                    <Icon
                      size="big"
                      name="angle left"
                      style={{ cursor: "pointer" }}
                      onClick={() => {
                        history.push(
                          `/submissions/page/${
                            parseInt(match.params.pageNumber) - 1
                          }`
                        );
                      }}
                    ></Icon>
                  )}
                  {submissions?.SubmissionsRemaining == true && (
                    <Icon
                      size="big"
                      name="angle right"
                      style={{ cursor: "pointer" }}
                      onClick={() => {
                        history.push(
                          `/submissions/page/${
                            parseInt(match.params.pageNumber) + 1
                          }`
                        );
                      }}
                    ></Icon>
                  )}
                </div>
              </Table.HeaderCell>
            </Table.Row>
          </Table.Footer>
        </Table>
      </Fragment>

      {activeCode != -1 && (
        <Modal open={true} onClose={() => setActiveCode(-1)} size="large">
          <Modal.Content>
            <SubmissonReport
              submissionId={String(submissions?.Submissions[activeCode].Id)}
              sourceCode={String(
                submissions?.Submissions[activeCode].SourceCode
              )}
            />
          </Modal.Content>
        </Modal>
      )}
    </Fragment>
  );
};

export default observer(SubmissionHistory);
