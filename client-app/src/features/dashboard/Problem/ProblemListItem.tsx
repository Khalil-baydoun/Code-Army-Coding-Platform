import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Button, Confirm, Label, Table } from "semantic-ui-react";
import { IProblemSummary } from "../../../app/models/problemSummary";
import { difficultyLabel } from "./ProblemsDashboard";

interface IProps {
  summary: IProblemSummary;
  hasRole: (role: string) => boolean | "" | null;
  deleteProblem: (id: string) => Promise<void>;
  submitting: boolean;
  target: string;
}

const ProblemListItem: React.FC<IProps> = ({
  summary,
  hasRole,
  deleteProblem,
  submitting,
  target,
}) => {
  const [opened, openConfirm] = useState(false);

  if (!summary) {
    return <div></div>;
  }
  // style={{ color: "#2f4858" }}
  return (
    <Table.Row>
      <Table.Cell width="1">
        <Link to={`problems/${summary.Id}`}>
          <p style={{ textDecoration: "none" }}>{summary.Id}</p>
        </Link>
      </Table.Cell>
      <Table.Cell width="4">
        <Link to={`problems/${summary.Id}`}>
          <p style={{ textDecoration: "none" }}>{summary.Title}</p>
        </Link>
      </Table.Cell>
      <Table.Cell width="2">{difficultyLabel(summary)}</Table.Cell>
      <Table.Cell width="9">
        {summary.Tags.map((tag: string, i) => (
          <Label key={i}>{tag}</Label>
        ))}
      </Table.Cell>
      {hasRole("Instructor") && (
        <Table.Cell width="1">
          <Button
            name={summary.Id}
            loading={target === summary.Id && submitting}
            onClick={() => openConfirm(true)}
            content="Delete"
            color="red"
            size="tiny"
          />
          <Confirm
            open={opened}
            header="This will permanently delete this problem and can't be undone!"
            confirmButton="Yes"
            size="tiny"
            onCancel={() => openConfirm(false)}
            onConfirm={() => {
              openConfirm(false);
              deleteProblem(summary.Id);
            }}
          />
        </Table.Cell>
      )}
    </Table.Row>
  );
};

export default ProblemListItem;
