import React from "react";
import { Link } from "react-router-dom";
import { Button, Card, Label } from "semantic-ui-react";
import { IProblemSetSummary } from "../../../app/models/courseProblemSet";

interface IProps {
  summary: IProblemSetSummary;
}

const ProblemSetListItem: React.FC<IProps> = ({ summary }) => {
  if (!summary) {
    return <div></div>;
  }
  return (
    <div style={{ marginRight: "90px", marginBottom: "50px", width: "40%" }}>
      <Card color="teal" fluid>
        <Card.Content>
          <Card.Header style={{ color: "#2f4858" }}>{summary.Name}</Card.Header>
          <Card.Description>
            {summary.Prerequisites.map((tag: string, i) => (
              <Label color="yellow" key={i}>
                {tag}
              </Label>
            ))}
          </Card.Description>
        </Card.Content>
        <Card.Content extra>
          <Button
            color="teal"
            floated="right"
            as={Link}
            to={`problemset/${summary.Id}/problems`}
          >
            Enter Problem Set
          </Button>
        </Card.Content>
      </Card>
    </div>
  );
};

export default ProblemSetListItem;
