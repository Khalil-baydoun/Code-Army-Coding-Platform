import { observer } from "mobx-react-lite";
import React, { useContext } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import {
  combineValidators,
  composeValidators,
  hasLengthGreaterThan,
  isRequired,
} from "revalidate";
import { Button, Container, Form } from "semantic-ui-react";
import SelectInput from "../../app/common/form/SelectInput";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import { progLangs } from "../../app/common/options/selectOptions";
import { RootStoreContext } from "../../app/stores/rootStore";

const validate = combineValidators({
  ProgLanguage: isRequired({ message: "The programming language is required" }),
  SourceCode: composeValidators(
    isRequired("Source Code"),
    hasLengthGreaterThan(3)({
      message: "Source Code needs to be at least 4 characters",
    })
  )(),
});

const SubmitForm = () => {
  const rootStore = useContext(RootStoreContext);
  const { Submit, problem } = rootStore.problemStore;

  const handleFinalFormSubmit = (values: any) => {
    const { ...request } = values;
    request.ProblemId = problem?.Id.toString();
    Submit(request);
  };

  if (!problem) {
    return <h2>Problem not found!</h2>;
  }

  return (
    problem && (
      <Container>
        <FinalForm
          validate={validate}
          onSubmit={handleFinalFormSubmit}
          render={({ handleSubmit, invalid, pristine }) => (
            <div style={{ paddingBottom: "30px" }}>
              <Form onSubmit={handleSubmit}>
                <Field
                  options={progLangs}
                  label="Language"
                  name="ProgLanguage"
                  placeholder="Language"
                  component={SelectInput}
                />
                <Field
                  label="Source Code"
                  name="SourceCode"
                  placeholder="Paste your code here..."
                  rows="20"
                  component={TextAreaInput}
                ></Field>

                {/* <Container textAlign="center"> */}
                <Button
                  disabled={invalid || pristine}
                  floated="right"
                  color="teal"
                  type="submit"
                  content="Submit"
                />
                {/* </Container> */}
              </Form>
            </div>
          )}
        />
      </Container>
    )
  );
};

export default observer(SubmitForm);
