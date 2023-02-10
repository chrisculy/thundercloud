import { Reducer } from "redux";
import { ActionType, createAction, getType } from 'typesafe-actions';

export const setGameId = createAction("GAME_SET_ID")<number>();

const GameActions = {
  setGameId
};

type GameAction = ActionType<typeof GameActions>;

export interface GameState {
  id?: number;
}

export const gameReducer: Reducer<GameState, GameAction> = (state: GameState = { id: undefined }, action: GameAction) => {
  switch (action.type) {
    case getType(GameActions.setGameId): {
      return { ...state, id: action.payload }
    }
  }

  return state;
}
