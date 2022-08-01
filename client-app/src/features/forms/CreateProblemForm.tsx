import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import { RouteComponentProps } from "react-router-dom";
import {
  combineValidators,
  composeValidators,
  createValidator,
  hasLengthGreaterThan,
  isRequired,
} from "revalidate";
import { Button, Form, Segment } from "semantic-ui-react";
import SelectInput from "../../app/common/form/SelectInput";
import SelectInputMultiple from "../../app/common/form/SelectInputMultiple";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import TextInput from "../../app/common/form/TextInput";
import { difficulities, tags } from "../../app/common/options/selectOptions";
import { ProblemFormValues } from "../../app/models/problem";
import { RootStoreContext } from "../../app/stores/rootStore";

const isPositiveNumber = createValidator(
  (message) => (value) => {
    if (value && Number(value) <= 0) {
      return message;
    }
  },
  (field) => `${field} must be greater than 0`
);

const validate = combineValidators({
  Title: isRequired({ message: "The problem title is required" }),
  GeneralDescription: composeValidators(
    isRequired("GeneralDescription"),
    hasLengthGreaterThan(19)({
      message: "General Description needs to be at least 20 characters",
    })
  )(),
  ODescription: composeValidators(
    isRequired("ODescription"),
    hasLengthGreaterThan(19)({
      message: "Output Description needs to be at least 20 characters",
    })
  )(),
  SampleInput: isRequired("Sample Input"),
  SampleOutput: isRequired("Sample Output"),
  TimeLimitInMilliseconds: isPositiveNumber("Time Limit"),
  MemoryLimitInKiloBytes: isPositiveNumber("Memory Limit"),
});

interface DetailParams {
  problemSetId: string;
  courseId: string;
}

const CreateProblemForm: React.FC<RouteComponentProps<DetailParams>> = ({
  match,
}) => {
  const rootStore = useContext(RootStoreContext);
  const { createProblem, submitting } = rootStore.problemStore;
  const {
    problemSet,
    loadProblemSet,
    loadingInitial,
  } = rootStore.courseProblemSetStore;
  const { user } = rootStore.userStore;
  const [problem, setProblem] = useState(new ProblemFormValues());
  useEffect(() => {
    loadProblemSet(match.params.courseId, match.params.problemSetId);
    setProblem(new ProblemFormValues());
  }, [loadProblemSet, match.params.courseId, match.params.problemSetId]);

  const handleFinalFormSubmit = (values: any) => {
    const {
      TimeLimitInMilliseconds,
      MemoryLimitInKiloBytes,
      TimeFactor,
      MemoryFactor,
      ...problem
    } = values;
    problem.TimeLimitInMilliseconds = parseInt(TimeLimitInMilliseconds);
    problem.MemoryLimitInKiloBytes = parseInt(MemoryLimitInKiloBytes);
    problem.TimeFactor = parseInt(TimeFactor);
    problem.MemoryFactor = parseInt(MemoryFactor);
    problem.AuthorEmail = user?.Email;
    problem.ProblemSetId = String(problemSet?.Id);
    let newProblem = {
      ...problem,
    };
    delete newProblem.Id;
    createProblem(newProblem);
  };

  if (!problemSet && !loadingInitial) {
    return <h2>Not inside a problem set!</h2>;
  }

  return (
    <Segment clearing>
      <h1>Problem Form</h1>
      <FinalForm
        validate={validate}
        initialValues={problem}
        onSubmit={handleFinalFormSubmit}
        render={({ handleSubmit, invalid, pristine }) => (
          <Form onSubmit={handleSubmit}>
            <Field
              name="Title"
              label="Title"
              placeholder="Insert Title here ..."
              value={problem.Title}
              component={TextInput}
            />
            <Field
              name="GeneralDescription"
              label="General Description"
              rows={7}
              placeholder="Insert General Problem Description here ..."
              value={problem.GeneralDescription}
              component={TextAreaInput}
            />
            <Field
              name="IDescription"
              label="Input Description"
              rows={4}
              placeholder="Insert Input Description here ..."
              value={problem.IDescription}
              component={TextAreaInput}
            />
            <Field
              name="ODescription"
              label="Output Description"
              rows={4}
              placeholder="Insert Output Description here ..."
              value={problem.ODescription}
              component={TextAreaInput}
            />
            <Form.Group inline>
              <Field
                options={difficulities}
                label="Difficulty"
                name="Difficulty"
                placeholder="Difficulty"
                value={problem.Difficulty}
                component={SelectInput}
              />
              <Field
                name="TimeLimitInMilliseconds"
                label="Time Limit (ms)"
                placeholder="TimeLimitInMilliseconds"
                type="number"
                value={problem.TimeLimitInMilliseconds.toString()}
                component={TextInput}
              />
              <Field
                name="MemoryLimitInKiloBytes"
                label="Memory Limit (KB)"
                placeholder="MemoryLimitInKiloBytes"
                type="number"
                component={TextInput}
                value={problem.MemoryLimitInKiloBytes.toString()}
              />
            </Form.Group>
            <Form.Group inline>
              <Field
                name="TimeFactor"
                label="Time Factor allowed"
                placeholder="TimeFactor"
                type="number"
                value={problem.TimeFactor.toString()}
                component={TextInput}
              />
              <Field
                name="MemoryFactor"
                label="Memory Factor Allowed"
                placeholder="MemoryFactor"
                type="number"
                component={TextInput}
                value={problem.MemoryFactor.toString()}
              />
            </Form.Group>
            <Field
              options={tags}
              label="Tags"
              name="Tags"
              placeholder="Tags"
              value={problem.Tags.join(",")}
              component={SelectInputMultiple}
            />
            <Form.Group inline>
              <Field
                width="15"
                name="SampleInput"
                label="Sample Input"
                rows={7}
                placeholder="Insert Sample Input here ..."
                value123={problem.SampleInput}
                component={TextAreaInput}
              />
              <Field
                width="15"
                name="SampleOutput"
                label="Sample Output"
                rows={7}
                placeholder="Insert Sample Output here ..."
                value123={problem.SampleOutput}
                component={TextAreaInput}
              />
            </Form.Group>
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

export default observer(CreateProblemForm);
