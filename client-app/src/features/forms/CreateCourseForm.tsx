import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import {
  combineValidators,
  composeValidators,
  hasLengthGreaterThan,
  isRequired,
} from "revalidate";
import { Button, Form, Segment } from "semantic-ui-react";
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

const CreateCourseForm: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { createCourse, submitting } = rootStore.courseProblemSetStore;
  const { user } = rootStore.userStore;
  const [course, setCourse] = useState(new CourseFormValues());
  useEffect(() => {
    setCourse(new CourseFormValues());
  }, [setCourse]);

  const handleFinalFormSubmit = (values: any) => {
    const { ...course } = values;
    course.AuthorEmail = user?.Email;
    let newCourse = {
      ...course,
    };
    delete newCourse.Id;
    createCourse(newCourse);
  };

  return (
    <Segment clearing>
      <h1>Course Form</h1>
      <FinalForm
        validate={validate}
        initialValues={course}
        onSubmit={handleFinalFormSubmit}
        render={({ handleSubmit, invalid, pristine }) => (
          <Form onSubmit={handleSubmit}>
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
              disabled={invalid || pristine}
              loading={submitting}
              floated="right"
              color="teal"
              type="submit"
              content="Submit"
            />
          </Form>
        )}
      />
    </Segment>
  );
};

export default observer(CreateCourseForm);
