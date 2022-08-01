import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import { RouteComponentProps } from "react-router-dom";
import {
  combineValidators,
  composeValidators,
  hasLengthGreaterThan,
  isRequired,
} from "revalidate";
import { Button, Form, Header, Label, Segment } from "semantic-ui-react";
import DateInput from "../../app/common/form/DateInput";
import SelectInputMultiple from "../../app/common/form/SelectInputMultiple";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import TextInput from "../../app/common/form/TextInput";
import { tags } from "../../app/common/options/selectOptions";
import { combineDateAndTime } from "../../app/common/util/util";
import { ProblemSetFormValues } from "../../app/models/courseProblemSet";
import { RootStoreContext } from "../../app/stores/rootStore";

const validate = combineValidators({
  Name: isRequired({ message: "The problem set name is required" }),
  Description: composeValidators(
    isRequired("Description"),
    hasLengthGreaterThan(9)({
      message: "Description needs to be at least 20 characters",
    })
  )(),
});

interface DetailParams {
  courseId: string;
}

const CreateProblemSetForm: React.FC<RouteComponentProps<DetailParams>> = ({
  match,
}) => {
  const rootStore = useContext(RootStoreContext);
  const {
    getCourse,
    getCourseGroups,
    loadingInitial,
    course,
    courseGroups,
    createProblemSet,
    submitting,
  } = rootStore.courseProblemSetStore;
  const { user } = rootStore.userStore;
  const [problemSet, setProblemSet] = useState(new ProblemSetFormValues());
  useEffect(() => {
    getCourse(match.params.courseId);
    getCourseGroups(match.params.courseId);
    setProblemSet(new ProblemSetFormValues());
  }, [setProblemSet, getCourse, match.params.courseId]);

  const handleFinalFormSubmit = (values: any) => {
    const { ...problemSet } = values;
    problemSet.AuthorEmail = user?.Email;
    problemSet.CourseId = course?.Id;
    let newProblemSet = {
      ...problemSet,
    };
    delete newProblemSet.Id;
    courseGroups?.forEach((element) => {
      if ("DueDate" + element.Id in values) {
        const dateAndTime = combineDateAndTime(
          values["DueDate" + element.Id],
          values["DueTime" + element.Id]
        );
        newProblemSet.dueDates.push({
          dueDate: dateAndTime,
          groupId: parseInt(element.Id),
          problemSetId: 0,
        });
      }
    });
    createProblemSet(newProblemSet);
  };

  if (!course && !loadingInitial) {
    return <h2>Not inside a course!</h2>;
  }

  return (
    <Segment clearing>
      <Header as="h2" textAlign="center">
        Problem Set Form
      </Header>
      <FinalForm
        validate={validate}
        initialValues={problemSet}
        onSubmit={handleFinalFormSubmit}
        render={({ handleSubmit, invalid, pristine }) => (
          <Form onSubmit={handleSubmit}>
            <Field
              name="Name"
              label="Name"
              placeholder="Insert problem set name here ..."
              value={problemSet.Name}
              component={TextInput}
            />
            <Field
              name="Description"
              label="Description"
              rows={5}
              placeholder="Insert the description of the problem set here"
              value={problemSet.Description}
              component={TextAreaInput}
            />
            <Field
              options={tags}
              label="Prerequisites"
              name="Prerequisites"
              placeholder="Prerequisites"
              value={problemSet.Prerequisites.join(",")}
              component={SelectInputMultiple}
            />
            {courseGroups?.map((x) => (
              <Segment key={x.Id}>
                <Label style={{ marginBottom: "10px" }}>
                  {x.Name + " group due date"}
                </Label>
                <Form.Group widths="equal">
                  <Field
                    name={"DueDate" + x.Id}
                    label={x.Name + " group due date"}
                    placeholder="Keep this empty if there is no due date for this group"
                    //value={}
                    date={true}
                    component={DateInput}
                  />
                  <Field
                    name={"DueTime" + x.Id}
                    label={x.Name + " group due time"}
                    placeholder="Keep this empty if there is no due date for this group"
                    //value={}
                    time={true}
                    component={DateInput}
                  />
                </Form.Group>
              </Segment>
            ))}

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

export default observer(CreateProblemSetForm);
