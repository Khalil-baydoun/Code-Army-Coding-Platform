import React, { useContext, useState } from "react";
import { Button, Dropdown, Input, Menu } from "semantic-ui-react";
import { difficultyOptions } from "../../app/common/options/selectOptions";
import { ISearchRequest } from "../../app/models/searchRequest";
import { RootStoreContext } from "../../app/stores/rootStore";

const SearchBox = () => {
  const rootStore = useContext(RootStoreContext);
  const { modifySearch, resetSearch } = rootStore.problemStore;
  const [request, setRequest] = useState<ISearchRequest>({
    Title: "",
    Difficulty: null,
  });

  const onSearchClick = () => {
    modifySearch(request);
  };

  const onResetClick = () => {
    setRequest({ Title: "", Difficulty: null });
    resetSearch(request);
  };

  const handleInputChange = (event: any, result: any) => {
    const { name, value } = result || event.target;
    setRequest({ ...request, [name]: value });
  };

  const handleNumberInputChange = (event: any, result: any) => {
    const { name, value } = result || event.target;
    setRequest({ ...request, [name]: parseInt(value) });
  };

  return (
    <Menu vertical inverted>
      <Menu.Item>
        <Input
          value={request.Title}
          name="Title"
          icon="search"
          placeholder="Search Problems..."
          onChange={handleInputChange}
          width="1"
        />
      </Menu.Item>
      <Menu.Item>
        <Dropdown
          key={new Date().getTime()}
          name="Difficulty"
          placeholder="Difficulty"
          options={difficultyOptions}
          fluid
          selection
          onChange={handleNumberInputChange}
          value={request.Difficulty === null ? undefined : request.Difficulty}
        />
      </Menu.Item>

      <Menu.Item>
        <Button.Group>
          <Button onClick={onResetClick}>Reset</Button>
          <Button.Or />
          <Button color="teal" onClick={onSearchClick}>
            Search
          </Button>
        </Button.Group>
      </Menu.Item>
    </Menu>
  );
};

export default SearchBox;
