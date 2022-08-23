import { date } from "date-fns/locale/zh-TW/index.js";
import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import {
  combineValidators,
  composeValidators,
  hasLengthGreaterThan,
  isRequired,
} from "revalidate";
import { Button, Form, Header, Label, Modal, Segment } from "semantic-ui-react";
import DateInput from "../../app/common/form/DateInput";
import SelectInputMultiple from "../../app/common/form/SelectInputMultiple";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import TextInput from "../../app/common/form/TextInput";
import { tags } from "../../app/common/options/selectOptions";
import { combineDateAndTime } from "../../app/common/util/util";
import {
  IProblemSet,
  ProblemSetFormValues,
} from "../../app/models/courseProblemSet";
import { RootStoreContext } from "../../app/stores/rootStore";

const validate = combineValidators({
  Name: isRequired({ message: "The problemSet name is required" }),
  Description: composeValidators(
    isRequired("Description"),
    hasLengthGreaterThan(9)({
      message: "Description needs to be at least 20 characters",
    })
  )(),
});

const EditProblemSetForm: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    updateProblemSet,
    submitting,
  } = rootStore.courseProblemSetStore;

  const [problemSet, setProblemSet] = useState<any>(new ProblemSetFormValues());
  const [showMessage, setShowMessage] = useState(false);
  useEffect(() => {
    if (rootStore.courseProblemSetStore.problemSet) {
      let ps:any = { ...rootStore.courseProblemSetStore.problemSet };
      setProblemSet(ps);
    }
  }, [rootStore.courseProblemSetStore.problemSet]);

  const handleFinalFormSubmit = (values: any) => {
    const { ...problemSet } = values;
    // const dateAndTime = combineDateAndTime(
    // values["DueDate"],
    // values["DueDate"]);
    problemSet.DueDate = new Date(values["DueDate"]);
    console.log(problemSet);
    updateProblemSet(problemSet).then(() => setShowMessage(true));
  };

  if (!rootStore.courseProblemSetStore.problemSet) {
    return <h2>Not inside a problemSet!</h2>;
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
            <Field
              value={problemSet.DueDate}
              name={"DueDate"}
              label={"due date"}
              placeholder="Keep this empty if there is no due date for this group"
              date={true}
              time={true}
              component={DateInput}
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
      <Modal
        open={showMessage}
        size="tiny"
        header="Success!"
        content={"ProblemSet update was successfull!"}
        onActionClick={() => setShowMessage(false)}
        actions={[{ key: "done", content: "Done", positive: true }]}
      />
    </Segment>
  );
};

export default observer(EditProblemSetForm);
