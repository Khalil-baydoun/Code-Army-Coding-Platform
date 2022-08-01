import React from "react";
import { Link } from "react-router-dom";
import { Button, Header, Icon, Segment } from "semantic-ui-react";

const Forbidden = () => {
  return (
    <Segment placeholder>
      <Header icon>
        <Icon name="warning" />
        Ops, 403 Forbidden!
      </Header>
      <Segment.Inline>
        <Button as={Link} to="/" primary color="teal">
          Return to Home page
        </Button>
      </Segment.Inline>
    </Segment>
  );
};

export default Forbidden;
