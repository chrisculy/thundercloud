import { combineReducers, Reducer } from "redux";
import { NationAction, nationReducer, NationState } from "./nation";
import { ObjectiveAction, objectiveReducer, ObjectiveState } from "./objective";

export interface GameState {
  nation: NationState;
  objectives: ObjectiveState;
}

export const gameReducer: Reducer<GameState, NationAction | ObjectiveAction> = combineReducers({
  nation: nationReducer,
  objectives: objectiveReducer,
});


