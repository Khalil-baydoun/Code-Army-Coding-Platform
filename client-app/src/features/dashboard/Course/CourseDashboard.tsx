import React, { Fragment } from "react";
import { Grid } from "semantic-ui-react";
import CourseList from "./CourseList";

const CourseDashboard: React.FC = () => {
  return (
    <Fragment>
      <Grid>
        <Grid.Row className="ProblemsCont">
          <Grid.Column width={16}>
            <CourseList />
          </Grid.Column>
        </Grid.Row>
      </Grid>
    </Fragment>
  );
};

export default (CourseDashboard);
