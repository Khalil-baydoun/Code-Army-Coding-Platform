import Editor from "@monaco-editor/react";
import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useRef, useState } from "react";
import { Container, Form, Header } from "semantic-ui-react";
import { progLangs } from "../../app/common/options/selectOptions";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { RootStoreContext } from "../../app/stores/rootStore";

const SolutionView = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    loadSolution,
    problem,
    solution,
    loadingInitial,
  } = rootStore.problemStore;

  const [progLang, changeProgLang] = useState(progLangs[0].value);

  useEffect(() => {
    loadSolution(problem!.Id, progLang); // Need to take input the prog lang
  }, [loadSolution, problem, progLang]);

  const monacoRef = useRef(null);

  const handleProgLangChange = (val: any) => {
    console.log(val);
    changeProgLang(parseInt(val));
  };

  function handleEditorWillMount(monaco: any) {
    // here is the monaco instance
    // do something before editor is mounted
    monaco.languages.typescript.javascriptDefaults.setEagerModelSync(true);
  }

  function handleEditorDidMount(editor: any, monaco: any) {
    // here is another way to get monaco instance
    // you can also store it in `useRef` for further usage
    monacoRef.current = editor;
    editor.getAction("editor.action.formatDocument").run();
    editor.trigger("anyString", "editor.action.formatDocument");
  }

  if (loadingInitial) return <LoadingComponent />;

  return (
    <Container>
      <Form style={{ marginBottom: "20px" }}>
        <Form.Select
          options={progLangs}
          label="Language"
          name="ProgLanguage"
          placeholder="Language"
          value={progLang}
          onChange={(e: any, { value }) => handleProgLangChange(value)}
        />
      </Form>
      {solution && (
        <Editor
          value={solution.replaceAll("\\r", "\n").replaceAll("\\n", "\n")}
          height="800px"
          theme="vs-dark"
          defaultLanguage="cpp"
          beforeMount={handleEditorWillMount}
          onMount={handleEditorDidMount}
          options={{
            acceptSuggestionOnCommitCharacter: true,
            acceptSuggestionOnEnter: "on",
            accessibilitySupport: "auto",
            autoIndent: true,
            automaticLayout: true,
            codeLens: true,
            colorDecorators: true,
            contextmenu: true,
            cursorBlinking: "blink",
            cursorSmoothCaretAnimation: false,
            cursorStyle: "line",
            disableLayerHinting: false,
            disableMonospaceOptimizations: false,
            dragAndDrop: false,
            fixedOverflowWidgets: false,
            folding: true,
            foldingStrategy: "auto",
            fontLigatures: false,
            formatOnPaste: true,
            formatOnType: true,
            hideCursorInOverviewRuler: false,
            highlightActiveIndentGuide: true,
            links: true,
            mouseWheelZoom: false,
            multiCursorMergeOverlapping: true,
            multiCursorModifier: "alt",
            overviewRulerBorder: true,
            overviewRulerLanes: 2,
            quickSuggestions: true,
            quickSuggestionsDelay: 100,
            readOnly: true,
            renderControlCharacters: false,
            renderFinalNewline: true,
            renderIndentGuides: true,
            renderLineHighlight: "all",
            renderWhitespace: "none",
            revealHorizontalRightPadding: 30,
            roundedSelection: true,
            rulers: [],
            scrollBeyondLastColumn: 5,
            scrollBeyondLastLine: true,
            selectOnLineNumbers: true,
            selectionClipboard: true,
            selectionHighlight: true,
            showFoldingControls: "mouseover",
            smoothScrolling: false,
            suggestOnTriggerCharacters: true,
            wordBasedSuggestions: true,
            wordSeparators: "~!@#$%^&*()-=+[{]}|;:'\",.<>/?",
            wordWrap: "off",
            wordWrapBreakAfterCharacters: "\t})]?|&,;",
            wordWrapBreakBeforeCharacters: "{([+",
            wordWrapBreakObtrusiveCharacters: ".",
            wordWrapColumn: 80,
            wordWrapMinified: true,
            wrappingIndent: "none",
          }}
        />
      )}
      {!solution && <Header as="h2">Soltuion not found</Header>}
    </Container>
  );
};

export default observer(SolutionView);
