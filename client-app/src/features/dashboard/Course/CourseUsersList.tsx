import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useState } from "react";
import { useForm } from "react-hook-form";
import {
  Form,
  Grid,
  GridColumn,
  Icon,
  List,
  Modal,
  Segment,
} from "semantic-ui-react";
import { RootStoreContext } from "../../../app/stores/rootStore";

const CourseUsersList: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { register, handleSubmit } = useForm();
  const [showMessage, setShowMessage] = useState(false);
  const [submittingFile, setSubmittingFile] = useState(false);

  const {
    course,
    submitting,
    addUsersToCourse,
  } = rootStore.courseProblemSetStore;

  const onSubmitSingle = async (data: any, event: any) => {
    if (data.user != null) {
      var resp = await addUsersToCourse([data.user]);
      event.target.reset();
      if (resp == true) {
        setShowMessage(true);
      }
    }
  };

  if (!course) {
    return <h1>Not in a course!</h1>;
  }

  return (
    <Fragment>
      <Segment style={{ fontSize: "15px" }}>
        <Grid container columns={2} verticalAlign="middle">
          <GridColumn width={6}>
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
                  loading={submitting && !submittingFile}
                  color="teal"
                  type="submit"
                  content="Add"
                ></Form.Button>
              </Form.Group>
            </Form>
          </GridColumn>
        </Grid>
        <Modal
          open={showMessage}
          size="tiny"
          header="Success!"
          content={"Addition of users was successfull!"}
          onActionClick={() => setShowMessage(false)}
          actions={[{ key: "done", content: "Done", positive: true }]}
        />
      </Segment>
      <h4>Course Students:</h4>
      <List divided animated verticalAlign="middle">
        {course.UsersEmails &&
          course.UsersEmails.map((email: string) => (
            <List.Item
              key={email}
              style={{ paddingTop: "15px", marginBottom: "10px" }}
            >
              <Icon name="user" />
              <List.Content>{email}</List.Content>
            </List.Item>
          ))}
      </List>
    </Fragment>
  );
};

export default observer(CourseUsersList);
