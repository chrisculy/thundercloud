import { Typography } from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { ObjectiveState } from "../state/objective";
import { StoreState } from "../state/store";

export interface ObjectiveViewProps extends ObjectiveState {
  dispatch: Dispatch
};

const ObjectiveView: React.FC<ObjectiveViewProps> = props => {
  return (
    <Grid container spacing={2} paddingY={8}>
      {
        props.objectives.map((x, index) => (
        <Grid xs={12} sm={4} key={`objective_${index+1}`}>
          <Typography variant="h6" display="flex" justifyContent="center">{`Objective ${index+1}`}</Typography>
        </Grid>)
        )
      }
      </Grid>
  );
}

const mapStateToProps = (state: StoreState) => {
  return {
    ...state.objectives
  };
};

const mapDispatchToProps = (dispatch: Dispatch) => {
	return {
    dispatch
  };
};

const ConnectedObjectiveView = connect(mapStateToProps, mapDispatchToProps)(ObjectiveView);

export { ConnectedObjectiveView as ObjectiveView };