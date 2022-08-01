import { observer } from "mobx-react-lite";
import React, { useContext, useState } from "react";
import { useForm } from "react-hook-form";
import { Form, Icon, Segment, Table } from "semantic-ui-react";
import { IProblemSummary } from "../../../app/models/problemSummary";
import { RootStoreContext } from "../../../app/stores/rootStore";

const UserProblemSetStatistics: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { register, handleSubmit } = useForm();
  const [userEmail, setUserEmail] = useState("");
  const { problemSet, didSolveProblem } = rootStore.courseProblemSetStore;

  const tableCellReturn = (problemId: string) => {
    if (didSolveProblem(userEmail, problemId) == true) {
      return (
        <Table.Cell width={2}>
          <Icon name="check" color="teal" size="large"></Icon>
        </Table.Cell>
      );
    } else {
      return (
        <Table.Cell width={2}>
          <Icon name="x" color="red" size="large"></Icon>
        </Table.Cell>
      );
    }
  };
  const onSubmitSingle = async (data: any) => {
    if (data.user != null) {
      setUserEmail(data.user);
    }
  };

  if (!problemSet) {
    return <h1>Not in a problemset!</h1>;
  }

  return (
    <Segment>
      <h5>Get the problems solved by each student in this problem set:</h5>
      <div style={{ fontSize: "15px" }}>
        <Form onSubmit={handleSubmit(onSubmitSingle)}>
          <Form.Group inline>
            <Form.Field>
              <label>User Email</label>
              <input
                type="text"
                ref={register}
                name="user"
                placeholder="User Email goes here"
              />
            </Form.Field>
            <Form.Button
              color="teal"
              type="submit"
              content="Check"
            ></Form.Button>
          </Form.Group>
        </Form>
      </div>

      {userEmail && userEmail.length > 0 && problemSet && (
        <div style={{ width: "30%" }}>
          <Table definition striped>
            <Table.Body>
              {problemSet.Problems.map((summary: IProblemSummary) => (
                <Table.Row key={summary.Id}>
                  <Table.Cell width={2}>{summary.Title}</Table.Cell>
                  {tableCellReturn(summary.Id)}
                </Table.Row>
              ))}
            </Table.Body>
          </Table>
        </div>
      )}
    </Segment>
  );
};

export default observer(UserProblemSetStatistics);
