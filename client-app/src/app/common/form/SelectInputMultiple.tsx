import React from "react";
import { FieldRenderProps } from "react-final-form";
import { Form, FormFieldProps, Label } from "semantic-ui-react";

interface IProps
  extends FieldRenderProps<string, HTMLSelectElement>,
    FormFieldProps {}

const SelectInputMultiple: React.FC<IProps> = ({
  input,
  width,
  options,
  placeholder,
  label,
  meta: { touched, error },
}) => {
  const vals = input.value
    ? (input.value + "").split(",").filter((x) => x.length > 0)
    : undefined;
  return (
    <Form.Field error={touched && !!error} width={width}>
      <Form.Select
        multiple={true}
        label={label}
        value={vals}
        onChange={(e, data) => input.onChange(data.value)}
        placeholder={placeholder}
        options={options}
      />
      {touched && error && (
        <Label basic color="red">
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default SelectInputMultiple;
