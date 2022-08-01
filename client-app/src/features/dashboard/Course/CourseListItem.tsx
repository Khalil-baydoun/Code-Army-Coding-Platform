import React from "react";
import { Link } from "react-router-dom";
import { Button, Card } from "semantic-ui-react";
import { ICourseSummary } from "../../../app/models/courseProblemSet";

interface IProps {
  summary: ICourseSummary;
}

const CourseListItem: React.FC<IProps> = ({ summary }) => {
  if (!summary) {
    return <div></div>;
  }
  return (
    <div style={{ marginRight: "90px", marginBottom: "50px", width: "40%" }}>
      <Card color="teal" fluid>
        <Card.Content>
          <Card.Header>{summary.Name}</Card.Header>
          <Card.Description>{summary.Description}</Card.Description>
        </Card.Content>
        <Card.Content extra>
          <Button
            color="teal"
            as={Link}
            floated="right"
            to={`/course/${summary.Id}/problemset`}
          >
            Enter Course
          </Button>
        </Card.Content>
      </Card>
    </div>
  );
};

export default CourseListItem;
