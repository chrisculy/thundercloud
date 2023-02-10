import { combineReducers, configureStore, Reducer } from "@reduxjs/toolkit";
import { gameReducer, GameState } from "./game";
import { NationAction, nationReducer, NationState } from "./nation";
import { ObjectiveState, ObjectiveAction, objectiveReducer } from "./objective";

export interface StoreState {
  game: GameState;
  objectives: ObjectiveState;
  nation: NationState;
}

export const storeReducer: Reducer<StoreState, NationAction | ObjectiveAction> = combineReducers({
  game: gameReducer,
  objectives: objectiveReducer,
  nation: nationReducer,
});

export const store = configureStore({ reducer: storeReducer });