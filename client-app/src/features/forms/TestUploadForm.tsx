import { observer } from "mobx-react-lite";
import React, { useContext, useState } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import {
  Button,
  Form,
  Modal,
  Segment,
  SemanticCOLORS,
} from "semantic-ui-react";
import { RootStoreContext } from "../../app/stores/rootStore";

const TestUploadForm = () => {
  const rootStore = useContext(RootStoreContext);

  const { submitting, uploadTests, problem } = rootStore.problemStore;
  const [showMessage, setShowMessage] = useState(false);

  const [inputButtonText, changeInputButtonText] = useState("Upload File");
  const [inputButtonColor, changeInputButtonColor] = useState<SemanticCOLORS>(
    "grey"
  );
  const [selectedInputFile, setSelectedInputFile] = useState<Blob>();

  const [outputButtonText, changeOutputButtonText] = useState("Upload File");
  const [outputButtonColor, changeOutputButtonColor] = useState<SemanticCOLORS>(
    "grey"
  );
  const [selectedOutputFile, setSelectedOutputFile] = useState<Blob>();

  const onFileUpload = (data: any) => {
    if (data.input != null && data.output != null && problem) {
      const formData = new FormData();
      formData.append("Input", selectedInputFile!, "Input");
      formData.append("Output", selectedOutputFile!, "Output");
      formData.append("ProblemId", problem.Id);
      uploadTests(formData).finally(() => {
        setShowMessage(true);
        changeInputButtonText("Upload File");
        changeInputButtonColor("grey");
        changeOutputButtonText("Upload File");
        changeOutputButtonColor("grey");
      });
    }
  };

  const handleChange = (event: any, Name: string) => {
    if (Name == "input") {
      changeInputButtonText(event.target.files[0].name);
      changeInputButtonColor("blue");
      setSelectedInputFile(event.target.files[0]);
    } else {
      changeOutputButtonText(event.target.files[0].name);
      changeOutputButtonColor("blue");
      setSelectedOutputFile(event.target.files[0]);
    }
  };

  if (!problem) {
    return <h2>Problem not found!</h2>;
  }

  return (
    <Segment clearing>
      <h1>Tests Upload</h1>
      <FinalForm
        onSubmit={onFileUpload}
        render={({ handleSubmit, invalid, pristine }) => (
          <div style={{ paddingBottom: "30px" }}>
            <Form onSubmit={handleSubmit}>
              <Segment>
                <div style={{ marginBottom: "10px" }}>
                  <h3>Input File:</h3>
                </div>
                <Button
                  as="label"
                  htmlFor="file1"
                  color={inputButtonColor}
                  content={inputButtonText}
                  icon="file alternate"
                  labelPosition="left"
                />
                <Field name="input" placeholder="Upload Input file">
                  {({ input }) => (
                    <input
                      {...input}
                      id="file1"
                      hidden
                      type="file"
                      name="input"
                      onChange={(e) => {
                        handleChange(e, "input");
                        input.onChange(e);
                      }}
                    ></input>
                  )}
                </Field>
              </Segment>
              <Segment>
                <div style={{ marginBottom: "10px" }}>
                  <h3>Output File:</h3>
                </div>
                <Button
                  as="label"
                  htmlFor="file2"
                  color={outputButtonColor}
                  content={outputButtonText}
                  icon="file alternate"
                  labelPosition="left"
                />
                <Field name="output" placeholder="Upload Output file">
                  {({ input }) => (
                    <input
                      {...input}
                      id="file2"
                      hidden
                      type="file"
                      name="output"
                      onChange={(e) => {
                        handleChange(e, "output");
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
        content={"Test upload was successfull!"}
        onActionClick={() => setShowMessage(false)}
        actions={[{ key: "done", content: "Done", positive: true }]}
      />
    </Segment>
  );
};

export default observer(TestUploadForm);
