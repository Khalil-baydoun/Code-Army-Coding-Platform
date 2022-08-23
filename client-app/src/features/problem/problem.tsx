import { observer } from "mobx-react-lite";
import React, { useContext, useEffect } from "react";
import { CopyToClipboard } from "react-copy-to-clipboard";
import { RouteComponentProps } from "react-router";
import { toast } from "react-toastify";
import {
  Container,
  Divider,
  Form,
  Grid,
  GridColumn,
  GridRow,
  Header,
  Icon,
  Label,
  Reveal,
  Segment,
  Tab,
  Table,
} from "semantic-ui-react";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { RootStoreContext } from "../../app/stores/rootStore";
import EditProblemForm from "../forms/EditProblemForm";
import SolutionUploadForm from "../forms/SolutionUploadForm";
import SubmitForm from "../forms/SubmitForm";
import TestUploadForm from "../forms/TestUploadForm";
import SolutionView from "./SolutionView";

interface DetailParams {
  problemId: string;
  problemSetId: string;
  courseId: string;
}

const ProblemDetails: React.FC<RouteComponentProps<DetailParams>> = ({
  match,
  history,
}) => {
  const rootStore = useContext(RootStoreContext);
  const { isAuthorizedToEditCourse } = rootStore.userStore;
  const {
    problem,
    loadProblem,
    loadingInitial,
  } = rootStore.problemStore;
  useEffect(() => {
    loadProblem(
      match.params.problemId,
      match.params.courseId,
      match.params.problemSetId
    );
    return () => {
    };
  }, [
    match.params.problemId,
    match.params.courseId,
    match.params.problemSetId,
    loadProblem,
  ]);

  if (loadingInitial) return <LoadingComponent />;

  if (!problem) {
    return <h2>Problem not found!</h2>;
  }
  const panes = [
    {
      menuItem: {
        key: "statement",
        content: "Problem Statement",
        icon: "clipboard",
      },
      render: () => (
        <Tab.Pane>
          <Grid>
            <GridRow>
              <Container textAlign="center">
                <h1>{problem.Title}</h1>
                <h4>Memory Limit : {problem.MemoryLimitInKiloBytes} KB</h4>
                <h4>Time Limit : {problem.TimeLimitInMilliseconds} ms</h4>
                <Divider style={{ marginBottom: "0px" }} />
              </Container>
            </GridRow>
            <GridRow className="problemRow" style={{ fontSize: "16px" }}>
              <GridColumn>
                <h3>Problem Description:</h3>
                <p>{problem.GeneralDescription}</p>
              </GridColumn>
            </GridRow>
            <GridRow className="problemRow" style={{ fontSize: "16px" }}>
              <GridColumn>
                <h3>Input:</h3>
                <p>{problem.InputDescription}</p>
              </GridColumn>
            </GridRow>
            <GridRow className="problemRow" style={{ fontSize: "16px" }}>
              <GridColumn>
                <h3>Output:</h3>
                <p>{problem.OutputDescription}</p>
              </GridColumn>
            </GridRow>

            <GridRow>
              <GridColumn>
                <Table celled style={{ borderColor: "444" }}>
                  <Table.Header>
                    <Table.Row textAlign="center">
                      <Table.HeaderCell>
                        <Header as="h3">
                          <Header.Content>Sample Input</Header.Content>
                        </Header>
                      </Table.HeaderCell>
                      <Table.HeaderCell>
                        <Header as="h3">
                          <Header.Content>Sample Output</Header.Content>
                        </Header>
                      </Table.HeaderCell>
                    </Table.Row>
                  </Table.Header>
                  <Table.Body>
                    <Table.Row>
                      <Table.Cell width="8">
                        <CopyToClipboard
                          text={problem.SampleInput}
                          onCopy={() => toast.info("Copied!")}
                        >
                          <Label as="a" color="teal" ribbon>
                            <Icon.Group>
                              <Icon name="clipboard" />
                            </Icon.Group>
                          </Label>
                        </CopyToClipboard>
                        <Form.TextArea
                          disabled={true}
                          value={problem.SampleInput}
                          style={{
                            padding: "10px",
                            wordSpacing: "5px",
                            lineHeight: "140%",
                            width: "100%",
                            height: "150px",
                            resize: "none",
                            fontSize: "14px",
                            fontWeight: "bold",
                          }}
                        />
                      </Table.Cell>
                      <Table.Cell>
                        <CopyToClipboard
                          text={problem.SampleOutput}
                          onCopy={() => toast.info("Copied!")}
                        >
                          <Label as="a" color="teal" ribbon="right">
                            <Icon.Group>
                              <Icon name="clipboard" />
                            </Icon.Group>
                          </Label>
                        </CopyToClipboard>
                        <Form.TextArea
                          disabled={true}
                          value={problem.SampleOutput}
                          style={{
                            padding: "10px",
                            fontWeight: "bold",
                            wordSpacing: "5px",
                            lineHeight: "140%",
                            width: "100%",
                            height: "150px",
                            resize: "none",
                            fontSize: "14px",
                          }}
                        />
                      </Table.Cell>
                    </Table.Row>
                  </Table.Body>
                </Table>
              </GridColumn>
            </GridRow>
            <GridRow>
              <Segment style={{ width: "100%" }}>
                {problem.Hints.map((hint: String, index) => (
                  <Reveal animated="move" instant style={{ width: "100%" }}>
                    <Reveal.Content visible style={{ width: "100%" }}>
                      <Segment style={{ width: "100%" }}>
                        <h3>Hint{index}</h3>
                      </Segment>
                    </Reveal.Content>
                    <Reveal.Content hidden>
                      <Segment>{hint}</Segment>
                    </Reveal.Content>
                  </Reveal>
                ))}
              </Segment>
            </GridRow>
          </Grid>
        </Tab.Pane>
      ),
    },
    {
      menuItem: {
        key: "solutionView",
        content: "Problem Solution",
        icon: "lightbulb",
      },
      render: () => (
        <Tab.Pane>
          <SolutionView />
        </Tab.Pane>
      ),
    },
    {
      menuItem: {
        key: "discussion",
        content: "Discussions",
        icon: "comments",
      },
      render: () => (
        <Tab.Pane>
        </Tab.Pane>
      ),
    },
    {
      menuItem: {
        key: "submit",
        content: "Submit",
        icon: "send",
      },
      render: () => (
        <Tab.Pane>
          <SubmitForm />
        </Tab.Pane>
      ),
    },
  ];
  if (isAuthorizedToEditCourse()) {
    panes.push(
      {
        menuItem: {
          key: "solutionUpload",
          content: "Solution Upload",
          icon: "lightbulb",
        },
        render: () => (
          <Tab.Pane>
            <SolutionUploadForm />
          </Tab.Pane>
        ),
      },
      {
        menuItem: {
          key: "manage",
          content: "Edit Problem",
          icon: "settings",
        },
        render: () => (
          <Tab.Pane>
            <EditProblemForm />
          </Tab.Pane>
        ),
      },

      {
        menuItem: {
          key: "tests",
          content: "Upload Tests",
          icon: "upload",
        },
        render: () => (
          <Tab.Pane>
            <TestUploadForm />
          </Tab.Pane>
        ),
      }
    );
  }
  const ProblemTabs = () => (
    <Tab menu={{ pointing: true, secondary: true }} panes={panes} />
  );

  return (
    <Segment>
      <ProblemTabs />
    </Segment>
  );
};

export default observer(ProblemDetails);
