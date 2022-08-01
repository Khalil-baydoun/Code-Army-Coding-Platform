import { observer } from "mobx-react-lite";
import React, { Fragment, useContext } from "react";
import { Link } from "react-router-dom";
import { Card, Header, Icon } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { ICourseSummary } from "../../../app/models/courseProblemSet";
import { RootStoreContext } from "../../../app/stores/rootStore";
import CourseListItem from "./CourseListItem";

const CourseList: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { user } = rootStore.userStore;
  const { hasRole } = rootStore.userStore;

  if (user == null || user.Courses == null) {
    return <LoadingComponent />;
  }

  return (
    <Fragment>
      <Header
        as="h2"
        icon="code branch"
        content={"My Courses"}
        style={{ marginBottom: "0", color: "#2f4858" }}
      />
      {/* <h2 style={{ marginBottom: "0" }}>My Courses</h2> */}
      {hasRole("Instructor") && (
        <Header
          as="h4"
          style={{ color: "#2f4858", textAlign: "right", marginTop: "0" }}
        >
          <Link
            to={"course/create"}
            style={{ textDecoration: "none", color: "black" }}
          >
            <Icon.Group size="large">
              <Icon name="plus" color="green" />
            </Icon.Group>
            Add course
          </Link>
        </Header>
      )}
      <hr style={{ marginBottom: "3em" }} />
      <Card.Group>
        {user.Courses &&
          user.Courses.map((summary: ICourseSummary) => (
            <CourseListItem key={summary.Id} summary={summary} />
          ))}
      </Card.Group>
    </Fragment>
  );
};

export default observer(CourseList);
