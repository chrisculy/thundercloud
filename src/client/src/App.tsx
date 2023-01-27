import { Container, Typography } from "@mui/material";
import { NationView } from "./components/NationView";
import { ObjectiveView } from "./components/ObjectiveView";

function App() {
  return (
    <Container maxWidth="lg">
      <Typography
        display="flex"
        justifyContent="center"
        paddingY={4}
        variant="h4"
      >
        thundercloud
      </Typography>
      <NationView />
      <ObjectiveView />
    </Container>
  );
}

export default App;
