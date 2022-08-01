import { observer } from "mobx-react-lite";
import React, { useContext, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import { Button, Form, Modal, Segment } from "semantic-ui-react";
import { SemanticCOLORS } from "semantic-ui-react/dist/commonjs/generic";
import SelectInput from "../../app/common/form/SelectInput";
import { progLangs } from "../../app/common/options/selectOptions";
import { RootStoreContext } from "../../app/stores/rootStore";

const SolutionUploadForm = () => {
  const rootStore = useContext(RootStoreContext);
  const [buttonText, changeButtonText] = useState("Upload File");
  const [buttonColor, changeButtonColor] = useState<SemanticCOLORS>("grey");
  const { submitting, uploadSolution, problem } = rootStore.problemStore;
  const [showMessage, setShowMessage] = useState(false);
  const [selectedSolutionFile, setSelectedSolutionFile] = useState<Blob>();

  const onFileUpload = (data: any) => {
    if (data.solution != null && problem) {
      const formData = new FormData();
      formData.append("SourceCode", selectedSolutionFile!, "Solution");
      formData.append("ProblemId", problem.Id);
      formData.append("ProgLanguage", data.ProgLanguage);
      uploadSolution(formData).finally(() => {
        setShowMessage(true);
        changeButtonText("Upload File");
        changeButtonColor("grey");
      });
    }
  };

  if (!problem) {
    return <h2>Problem not found!</h2>;
  }

  const handleChange = (event: any) => {
    changeButtonText(event.target.files[0].name);
    changeButtonColor("teal");
    setSelectedSolutionFile(event.target.files[0]);
  };

  return (
    <Segment clearing>
      <h1>Problem Solution Upload</h1>
      <FinalForm
        onSubmit={onFileUpload}
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
              <Segment>
                <div style={{ marginBottom: "10px" }}>
                  <h3>Solution File:</h3>
                </div>
                <Button
                  as="label"
                  htmlFor="file"
                  color={buttonColor}
                  content={buttonText}
                  icon="file code"
                  labelPosition="left"
                />
                <Field name="solution" placeholder="Upload Solution file">
                  {({ input }) => (
                    <input
                      id="file"
                      hidden
                      type="file"
                      name="solution"
                      onChange={(e) => {
                        handleChange(e);
                        input.onChange(e);
                      }}
                    ></input>
                  )}
                </Field>
              </Segment>
              <Button
                loading={submitting}
                floated="left"
                color="teal"
                type="submit"
                content="Submit"
              />
            </Form>
          </div>
        )}
      />
      <Modal
        open={showMessage}
        size="tiny"
        header="Success!"
        content={"Solution upload was successfull!"}
        onActionClick={() => setShowMessage(false)}
        actions={[{ key: "done", content: "Done", positive: true }]}
      />
    </Segment>
  );
};

export default observer(SolutionUploadForm);
