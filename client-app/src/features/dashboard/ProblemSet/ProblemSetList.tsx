import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useEffect, useState } from "react";
import { Link, RouteComponentProps } from "react-router-dom";
import { Card, Confirm, Header, Icon } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { IProblemSetSummary } from "../../../app/models/courseProblemSet";
import { RootStoreContext } from "../../../app/stores/rootStore";
import ProblemSetListItem from "./ProblemSetListItem";

interface DetailParams {
  id: string;
}

const ProblemSetList: React.FC<RouteComponentProps<DetailParams>> = ({
  match,
}) => {
  const rootStore = useContext(RootStoreContext);
  const {
    getCourse,
    loadingInitial,
    course,
    problemSetRegistry,
    deleteCourse,
  } = rootStore.courseProblemSetStore;
  const { isAuthorizedToEditCourse, hasRole } = rootStore.userStore;
  const [opened, openConfirm] = useState(false);
  useEffect(() => {
    getCourse(match.params.id);
  }, [getCourse, match.params.id]);

  if (loadingInitial || course == null || problemSetRegistry == null) {
    return <LoadingComponent />;
  }

  return (
    <Fragment>
      <div>
        <Header
          as="h2"
          icon="student"
          style={{ color: "#2f4858" }}
          content={course.Name}
        />
        {isAuthorizedToEditCourse() && (
          <Fragment>
            <Header
              as="h4"
              style={{
                marginTop: "0",
                textAlign: "right",
              }}
            >
              <Link
                to={"problemset/create"}
                style={{ textDecoration: "none", color: "#2f4858" }}
              >
                <Icon.Group size="large">
                  <Icon name="plus" color="green" />
                </Icon.Group>
                Add ProblemSet
              </Link>
              <Link
                to={"manage"}
                style={{
                  marginLeft: "15px",
                  textDecoration: "none",
                  color: "#2f4858",
                }}
              >
                <Icon.Group size="large">
                  <Icon name="edit" color="teal" />
                </Icon.Group>
                Manage Course
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
                    Delete Course
                  </span>
                  <Confirm
                    open={opened}
                    header="Deleting this course will delete corresponding problem sets and problems, this can't be undone"
                    confirmButton="Yes"
                    style={{ color: "#2f4858" }}
                    size="tiny"
                    onCancel={() => openConfirm(false)}
                    onConfirm={() => {
                      openConfirm(false);
                      deleteCourse(course.Id);
                    }}
                  />
                </Fragment>
              )}
            </Header>
          </Fragment>
        )}
      </div>
      <hr style={{ marginBottom: "3em" }} />
      <Card.Group>
        {problemSetRegistry &&
          Array.from(problemSetRegistry.values())
            .filter((ps) => ps.CourseId == course.Id)
            .map((summary: IProblemSetSummary) => (
              <ProblemSetListItem summary={summary} />
            ))}
      </Card.Group>
    </Fragment>
  );
};

export default observer(ProblemSetList);
