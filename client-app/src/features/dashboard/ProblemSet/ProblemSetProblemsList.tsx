import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useEffect, useState } from "react";
import { Link, RouteComponentProps } from "react-router-dom";
import { Confirm, Header, Icon, List, Segment, Table } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { IProblemSummary } from "../../../app/models/problemSummary";
import { RootStoreContext } from "../../../app/stores/rootStore";
import ProblemListItem from "../Problem/ProblemListItem";

interface DetailParams {
  courseId: string;
  problemSetId: string;
}

const ProblemSetProblemsList: React.FC<RouteComponentProps<DetailParams>> = ({
  match,
}) => {
  const rootStore = useContext(RootStoreContext);
  const {
    problemSummariesById,
    submitting,
    target,
    deleteProblem,
  } = rootStore.problemStore;
  const {
    loadProblemSet,
    problemSet,
    loadingInitial,
    deleteProblemSet,
  } = rootStore.courseProblemSetStore;
  const [opened, openConfirm] = useState(false);
  const { isAuthorizedToEditCourse, hasRole } = rootStore.userStore;
  useEffect(() => {
    console.log("loading");
    loadProblemSet(match.params.courseId, match.params.problemSetId);
  }, [match.params.courseId, match.params.problemSetId, loadProblemSet]);

  if (loadingInitial || problemSet == null) {
    return <LoadingComponent />;
  }

  return (
    <Fragment>
      <Header
        as="h2"
        icon="laptop"
        style={{ marginBottom: "0", color: "#2f4858" }}
        content={problemSet.Name}
      />
      {isAuthorizedToEditCourse() && (
        <Fragment>
          <Header as="h4" style={{ textAlign: "right", marginTop: "0" }}>
            <Link
              to={"create"}
              style={{
                textDecoration: "none",
                cursor: "pointer",
                color: "#2f4858",
              }}
            >
              <Icon.Group size="large">
                <Icon name="plus" color="green" />
              </Icon.Group>
              Add Problem
            </Link>
            <Link
              to={"manage"}
              style={{
                textDecoration: "none",
                cursor: "pointer",
                marginLeft: "15px",
                color: "#2f4858",
              }}
            >
              <Icon.Group size="large">
                <Icon name="edit" color="teal" />
              </Icon.Group>
              Manage ProblemSet
            </Link>
            {hasRole("Instructor") && (
              <Fragment>
                <span
                  onClick={() => openConfirm(true)}
                  style={{
                    cursor: "pointer",
                    marginLeft: "15px",
                    textDecoration: "none",
                    color: "#2f4858",
                  }}
                >
                  <Icon.Group size="large">
                    <Icon name="edit" color="red" />
                  </Icon.Group>
                  Delete Problemset
                </span>
                <Confirm
                  open={opened}
                  header="Deleting this problem set will delete corresponding problems, this can't be undone"
                  confirmButton="Yes"
                  size="tiny"
                  onCancel={() => openConfirm(false)}
                  onConfirm={() => {
                    openConfirm(false);
                    deleteProblemSet(problemSet.Id, problemSet.CourseId);
                  }}
                />
              </Fragment>
            )}
          </Header>
          <Header
            as="h4"
            style={{ textAlign: "right", marginTop: "0" }}
          ></Header>
        </Fragment>
      )}
      <hr style={{ marginBottom: "3em" }} />

      <Segment style={{ color: "#2f4858" }}>
        <h3>Problem Set Overview:</h3>
        <p style={{ fontSize: "16px", marginBottom: "1.5em" }}>
          {problemSet.Description}
        </p>
        <h3>Prerequisites:</h3>
        <List bulleted>
          {problemSet.Prerequisites.map((tag: string, i) => (
            <List.Item key={i}>{tag}</List.Item>
          ))}
        </List>
        <Table celled selectable>
          <Table.Header>
            <Table.Row>
              <Table.HeaderCell
                colSpan={(hasRole("Instructor") && "5") || "4"}
                textAlign="center"
                style={{ fontSize: "20px" }}
              >
                Problems List
              </Table.HeaderCell>
            </Table.Row>
            <Table.Row>
              <Table.HeaderCell>ID </Table.HeaderCell>
              <Table.HeaderCell>Name</Table.HeaderCell>
              <Table.HeaderCell>Difficulty</Table.HeaderCell>
              <Table.HeaderCell>Tags</Table.HeaderCell>
              {hasRole("Instructor") && <Table.HeaderCell></Table.HeaderCell>}
            </Table.Row>
          </Table.Header>
          <Table.Body>
            {problemSummariesById.map((summary: IProblemSummary) => (
              <ProblemListItem
                key={summary.Id}
                summary={summary}
                submitting={submitting}
                target={target}
                deleteProblem={deleteProblem}
                hasRole={hasRole}
              />
            ))}
          </Table.Body>
          <Table.Footer>
            <Table.Row>
              <Table.HeaderCell
                colSpan={(hasRole("Instructor") && "5") || "4"}
              ></Table.HeaderCell>
            </Table.Row>
          </Table.Footer>
        </Table>
      </Segment>
    </Fragment>
  );
};

export default observer(ProblemSetProblemsList);
