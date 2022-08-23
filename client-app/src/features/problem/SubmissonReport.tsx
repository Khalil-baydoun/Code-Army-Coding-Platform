import { observer } from "mobx-react-lite";
import React, { useContext, useEffect } from "react";
import SyntaxHighlighter from "react-syntax-highlighter";
import {
  Container,
  Divider,
  Grid,
  GridRow,
  Header,
  Icon,
  Label,
  Segment,
} from "semantic-ui-react";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { RootStoreContext } from "../../app/stores/rootStore";

export function progLangToString(lang: any) {
  let langs = ["Cpp", "Java", "Python"];
  return langs[lang];
}

interface DetailParams {
  submissionId: string;
  sourceCode: string;
}

const SubmissionReport: React.FC<DetailParams> = ({
  submissionId,
  sourceCode,
}) => {
  const rootStore = useContext(RootStoreContext);
  const { report, loadingInitial, LoadReport } = rootStore.problemStore;

  useEffect(() => {
    LoadReport(submissionId);
  }, [submissionId, LoadReport]);
  const WAReport = () => {
    if (report?.WaReport != null) {
      let input = report?.WaReport.Input.split("\r\n").map((str, i) => (
        <p style={{ fontSize: "13px" }} key={"inp" + i}>
          {str}
        </p>
      ));
      let expout = report?.WaReport.ExpectedOutput.split("\r\n").map(
        (str, i) => (
          <p style={{ fontSize: "13px" }} key={"exp" + i}>
            {str}
          </p>
        )
      );
      let actout = report?.WaReport.ActualOutput.split("\r\n").map((str, i) => (
        <p style={{ fontSize: "13px" }} key={"act" + i}>
          {str}
        </p>
      ));
      return (
        <Segment>
          <Divider horizontal>
            <Header as="h4">
              <Icon name="exclamation triangle" color="red" />
              Wrong Test
            </Header>
          </Divider>
          <div>
            <Segment>
              <h4>Input:</h4>
              {input}
            </Segment>
            <Segment>
              <h4>Expected Output:</h4>
              {expout}
            </Segment>
            <Segment>
              <h4>Actual Output:</h4>
              {actout}
            </Segment>
          </div>
        </Segment>
      );
    }
  };

  if (loadingInitial || !report) {
    return <LoadingComponent />;
  }

  return (
    <Segment>
      <Header
        as="h2"
        content={`Submission Report #${submissionId}`}
        style={{ marginBottom: "0", color: "#2f4858" }}
      />

      <Grid>
        <GridRow>
          <Container>
            <Segment>
              <Segment>
                <Divider horizontal>
                  <Header as="h4">
                    <Icon name="code" color="teal" />
                    Source Code
                  </Header>
                </Divider>
                <SyntaxHighlighter language={"python"}>
                  {sourceCode}
                </SyntaxHighlighter>
              </Segment>
              {WAReport()}
            </Segment>
            <Segment textAlign="center">
              <Label style={{ fontSize: "12pt" }}>
                <Icon name="lightbulb outline" color="yellow"></Icon>
                <strong>
                  To know more about the verdict you got, check the verdicts
                  guide in the navbar!
                </strong>
              </Label>
            </Segment>
          </Container>
        </GridRow>
      </Grid>
    </Segment>
  );
};

export default observer(SubmissionReport);
