import { Box, Stack } from "@mui/material";
import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Unstable_Grid2/Grid2";
import { Dispatch } from "@reduxjs/toolkit";
import { connect } from "react-redux";
import { GuildView } from "./GuildView";
import { GameState } from "../state/game";
import { NationState } from "../state/nation";

export interface NationViewProps extends NationState {
  dispatch: Dispatch
};

const NationView: React.FC<NationViewProps> = (props) => {
  return (
    <Paper>
      <Box padding={4}>
        <Grid container spacing={2}>
          <Grid xs={12} sm={6} md={3}>
            <GuildView {...props.guilds.Agriculture} isActive={props.activeGuild === 'Agriculture'}  />
          </Grid>
          <Grid xs={12} sm={6} md={3}>
            <GuildView {...props.guilds.Industry} isActive={props.activeGuild === 'Industry'} />
          </Grid>
          <Grid xs={12} sm={6} md={3}>
            <GuildView {...props.guilds.Research} isActive={props.activeGuild === 'Research'} />
          </Grid>
          <Grid xs={12} sm={6} md={3}>
            <GuildView {...props.guilds.Energy} isActive={props.activeGuild === 'Industry'} />
          </Grid>
        </Grid>
        <Typography variant="h5" display="flex" justifyContent="center" paddingTop={4}>Resources</Typography>
        <Stack direction="row" justifyContent="center" display="flex" paddingTop={2} paddingBottom={4}>
          <Stack padding={2}>
            <Typography variant="h6">{`${
              props.resources.wood ?? 0
            } wood`}</Typography>
            <Typography variant="h6">{`${
              props.resources.stone ?? 0
            } stone`}</Typography>
          </Stack>
          <Stack padding={2}>
            <Typography variant="h6">{`${
              props.resources.food ?? 0
            } food`}</Typography>
            <Typography variant="h6">{`${
              props.resources.superfood ?? 0
            } superfood`}</Typography>
          </Stack>
          <Stack padding={2}>
            <Typography variant="h6">{`${
              props.resources.knowledge ?? 0
            } knowledge`}</Typography>
            <Typography variant="h6">{`${
              props.resources.stormknowledge ?? 0
            } stormknowledge`}</Typography>
          </Stack>
          <Stack padding={2}>
            <Typography variant="h6">{`${
              props.resources.power ?? 0
            } power`}</Typography>
            <Typography variant="h6">{`${
              props.resources.stormpower ?? 0
            } stormpower`}</Typography>
          </Stack>
        </Stack>
      </Box>
    </Paper>
  );
};

const mapStateToProps = (state: GameState) => {
  return {
    ...state.nation
  }
}

const mapDispatchToProps = (dispatch: Dispatch) => {
	return {
    dispatch
  };
};

const ConnectedNationView = connect(mapStateToProps, mapDispatchToProps)(NationView);

export { ConnectedNationView as NationView };