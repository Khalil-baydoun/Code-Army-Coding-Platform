import React from "react";
import { Link } from "react-router-dom";
import { Button, Header, Icon, Segment } from "semantic-ui-react";

const NotFound = () => {
  return (
    <Segment placeholder>
      <Header icon>
        <Icon name="search" />
        Ops, 404 Not Found!
      </Header>
      <Segment.Inline>
        <Button as={Link} to="/" primary color="teal">
          Return to Home page
        </Button>
      </Segment.Inline>
    </Segment>
  );
};

export default NotFound;
