import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import {
  combineValidators,
  composeValidators,
  hasLengthGreaterThan,
  isRequired,
} from "revalidate";
import { Button, Form, Modal, Segment } from "semantic-ui-react";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import TextInput from "../../app/common/form/TextInput";
import { CourseFormValues } from "../../app/models/courseProblemSet";
import { RootStoreContext } from "../../app/stores/rootStore";

const validate = combineValidators({
  Name: isRequired({ message: "The course name is required" }),
  Description: composeValidators(
    isRequired("Description"),
    hasLengthGreaterThan(9)({
      message: "Description needs to be at least 20 characters",
    })
  )(),
});

const EditCourseForm: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { updateCourse, submitting } = rootStore.courseProblemSetStore;
  const [loading, setLoading] = useState(false);
  const [course, setCourse] = useState(new CourseFormValues());
  const [showMessage, setShowMessage] = useState(false);
  useEffect(() => {
    if (rootStore.courseProblemSetStore.course) {
      setLoading(true);
      setCourse(new CourseFormValues(rootStore.courseProblemSetStore.course));
      setLoading(false);
    }
  }, [rootStore.courseProblemSetStore.course]);

  const handleFinalFormSubmit = (values: any) => {
    const { ...course } = values;
    updateCourse(course).finally(() => setShowMessage(true));
  };

  if (!rootStore.courseProblemSetStore.course) {
    return <h2>Not inside a course!</h2>;
  }
  return (
    <Segment clearing>
      <h1>Course Form</h1>
      <FinalForm
        validate={validate}
        initialValues={course}
        onSubmit={handleFinalFormSubmit}
        render={({ handleSubmit, invalid, pristine }) => (
          <Form onSubmit={handleSubmit} loading={loading}>
            <Field
              name="Name"
              label="Name"
              placeholder="Insert course name here ..."
              value={course.Name}
              component={TextInput}
            />
            <Field
              name="Description"
              label="Description"
              rows={5}
              placeholder="Insert the description of the course here"
              value={course.Description}
              component={TextAreaInput}
            />
            <Button
              disabled={loading || invalid || pristine}
              loading={submitting}
              floated="right"
              color="teal"
              type="submit"
              content="Submit"
            />
          </Form>
        )}
      />
      <Modal
        open={showMessage}
        size="tiny"
        header="Success!"
        content={"Course update was successfull!"}
        onActionClick={() => setShowMessage(false)}
        actions={[{ key: "done", content: "Done", positive: true }]}
      />
    </Segment>
  );
};

export default observer(EditCourseForm);
