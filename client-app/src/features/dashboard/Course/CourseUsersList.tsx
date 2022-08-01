import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import { useForm } from "react-hook-form";
import {
  Button,
  Form,
  Grid,
  GridColumn,
  Icon,
  List,
  Modal,
  Segment,
} from "semantic-ui-react";
import { SemanticCOLORS } from "semantic-ui-react/dist/commonjs/generic";
import { RootStoreContext } from "../../../app/stores/rootStore";

const CourseUsersList: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { register, handleSubmit } = useForm();
  const [showMessage, setShowMessage] = useState(false);

  const [buttonText, changeButtonText] = useState("Upload File");
  const [buttonColor, changeButtonColor] = useState<SemanticCOLORS>("grey");
  const [selectedUsersFile, setSelectedUsersFile] = useState<Blob>();
  const [submittingFile, setSubmittingFile] = useState(false);

  const handleChange = (event: any) => {
    changeButtonText(event.target.files[0].name);
    changeButtonColor("blue");
    setSelectedUsersFile(event.target.files[0]);
  };

  const {
    course,
    submitting,
    addUserToCourse,
    addUsersToCourse,
  } = rootStore.courseProblemSetStore;

  const onSubmitSingle = async (data: any, event: any) => {
    if (data.user != null) {
      var resp = await addUserToCourse(data.user);
      event.target.reset();
      if (resp == true) {
        setShowMessage(true);
      }
    }
  };
  const onSubmitFile = async (data: any) => {
    if (data.users != null) {
      const formData = new FormData();
      formData.append("users", selectedUsersFile!, "users");
      formData.append("CourseId", course?.Id!);
      setSubmittingFile(true);
      var resp = await addUsersToCourse(formData);
      if (resp == true) {
        setShowMessage(true);
      }
      changeButtonText("Upload File");
      changeButtonColor("grey");
      setSubmittingFile(false);
    }
  };

  if (!course) {
    return <h1>Not in a course!</h1>;
  }

  return (
    <Fragment>
      <Segment style={{ fontSize: "15px" }}>
        <p>
          <strong>Note: </strong>Add users individually by email, or through a
          file upload containing user emails one on each line.
        </p>
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
          <GridColumn width={10}>
            <FinalForm
              onSubmit={onSubmitFile}
              render={({ handleSubmit, form }) => (
                <div style={{ paddingBottom: "30px" }}>
                  <Form onSubmit={handleSubmit}>
                    <Segment>
                      <div style={{ marginBottom: "10px" }}>
                        <h3>Users File:</h3>
                      </div>
                      <Button
                        as="label"
                        htmlFor="file"
                        color={buttonColor}
                        content={buttonText}
                        icon="user"
                        labelPosition="left"
                      />
                      <Field name="users" placeholder="Upload Users file">
                        {({ input }) => (
                          <input
                            id="file"
                            hidden
                            type="file"
                            name="users"
                            onChange={(e) => {
                              handleChange(e);
                              input.onChange(e);
                            }}
                          ></input>
                        )}
                      </Field>
                      <Button
                        loading={submitting && submittingFile}
                        color="teal"
                        type="submit"
                        content="Submit"
                      />
                    </Segment>
                  </Form>
                </div>
              )}
            />
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
