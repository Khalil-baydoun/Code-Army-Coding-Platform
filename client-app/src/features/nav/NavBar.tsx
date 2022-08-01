import { observer } from "mobx-react-lite";
import React, { useContext } from "react";
import { Link } from "react-router-dom";
import { Container, Dropdown, Menu } from "semantic-ui-react";
import { RootStoreContext } from "../../app/stores/rootStore";

const NavBar = () => {
  const rootStore = useContext(RootStoreContext);
  const { user, logout } = rootStore.userStore;

  const SettingsDropdown = () => (
    <Dropdown
      style={{ color: "#ffd62f", fontSize: "12pt" }}
      trigger={<span>{user?.FirstName}</span>}
    >
      <Dropdown.Menu>
        <Dropdown.Item
          color="#ffd62f"
          icon="pie chart"
          text="Your Statistics"
          as={Link}
          to="/statistics"
        />
        <Dropdown.Item
          color="#ffd62f"
          text="Your Submissions"
          icon="code"
          as={Link}
          to="/submissions/page/1"
        />
        <Dropdown.Item
          color="#ffd62f"
          text="Verdicts Guide"
          icon="help"
          as={Link}
          to="/verdictsGuide"
        />
        <Dropdown.Item onClick={logout} text="Logout" icon="power" />
      </Dropdown.Menu>
    </Dropdown>
  );

  return (
    <Menu fixed="top" inverted className="Nav">
      <Container className="hcont">
        <Menu.Item header as={Link} exact="true" to="/course">
          <img
            src="/assets/images/logo3.png"
            alt="logo"
            style={{ marginRight: "10px" }}
          />
          <span
            style={{
              fontWeight: 300,
              letterSpacing: "1px",
              color: "#ffd62f",
              fontSize: "120%",
            }}
          >
            AUB CODING PLATFORM
          </span>
        </Menu.Item>

        <Menu.Menu position="right">
          <Menu.Item>{user && <SettingsDropdown />}</Menu.Item>
        </Menu.Menu>
      </Container>
    </Menu>
  );
};
export default observer(NavBar);
