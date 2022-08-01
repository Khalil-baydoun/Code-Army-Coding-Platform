import { observer } from "mobx-react-lite";
import React, { Fragment } from "react";
import { Grid, Label } from "semantic-ui-react";
import { difficultyToString } from "../../../app/common/util/commanData";
import { IProblemSummary } from "../../../app/models/problemSummary";
import SearchBox from "../../searchBox/SearchBox";
import ProblemsList from "./ProblemsList";

export function difficultyLabel(summary: IProblemSummary) {
  let diffColors = ["green", "orange", "red"] as const;
  return (
    <Label as="a" color={diffColors[summary.Difficulty]} tag>
      {difficultyToString(summary.Difficulty)}
    </Label>
  );
}

const ProblemsDashboard: React.FC = () => {
  return (
    <Fragment>
      <Grid>
        <Grid.Row className="ProblemsCont">
          <Grid.Column width={14}>
            <ProblemsList />
          </Grid.Column>
          <Grid.Column width={2}>
            <SearchBox />
          </Grid.Column>
        </Grid.Row>
      </Grid>
    </Fragment>
  );
};

export default observer(ProblemsDashboard);
