import { Construction, KeyboardArrowUp, KeyboardDoubleArrowUp, Work } from "@mui/icons-material";
import {
  Button,
  ButtonGroup,
  Card,
  CardActions,
  CardContent,
  Stack,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2/Grid2";
import { Guild } from "../models/Guild";
import { resourceSetToStrings } from "../models/ResourceSet";

export interface GuildViewProps extends Guild
{
  isActive: boolean;
}

export const GuildView: React.FC<GuildViewProps> = (props) => {
  return (
    <Card>
      <CardContent>
        <Stack>
          <Typography variant="h5">{props.name}</Typography>
          <Stack direction="row">
            <Stack>
              <Typography variant="body1">Building Cost (Common):</Typography>
              {resourceSetToStrings(props.buildingCostCommon).map((x, index) => (
                <Typography variant='body2' key={`buildingCostCommonResource${index}`}>{x}</Typography>
              ))}
            </Stack>
          </Stack>
          <Stack direction="row">
            <Stack>
              <Typography variant="body1">Building Cost (Rare):</Typography>
              {resourceSetToStrings(props.buildingCostRare).map((x, index) => (
                <Typography variant='body2' key={`buildingCostRareResource${index}`}>{x}</Typography>
              ))}
            </Stack>
          </Stack>
          <Stack>
            <Typography variant="body1">Production Rate (Common):</Typography>
            <Typography variant='body2'>{props.productionRateCommon}</Typography>
          </Stack>
          <Stack>
            <Typography variant="body1">Production Rate (Rare):</Typography>
            <Typography variant='body2'>{props.productionRateRare}</Typography>
          </Stack>
        </Stack>
      </CardContent>
      <CardActions>
        <Grid container spacing={4}>
          <Grid>
            <ButtonGroup variant='contained' size='small' color='success' orientation='vertical'>
              <Button variant='outlined'><Work /></Button>
              <Button><Work /></Button>
            </ButtonGroup>
          </Grid>
          <Grid>
            <ButtonGroup variant='contained' size='small' color='primary' orientation='vertical'>
              <Button variant='outlined'><Construction /></Button>
              <Button><Construction /></Button>
            </ButtonGroup>
          </Grid>
          <Grid>
            <ButtonGroup variant='contained' size='small' color='secondary' orientation='vertical'>
              <Button variant='outlined'><KeyboardArrowUp /></Button>
              <Button><KeyboardDoubleArrowUp /></Button>
            </ButtonGroup>
          </Grid>
        </Grid>
      </CardActions>
    </Card>
  );
};
