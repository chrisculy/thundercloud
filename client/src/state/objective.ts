import { Reducer } from "redux";
import { Objective } from "../models/Objective";
import { ActionType, createAction, getType } from "typesafe-actions";

export const queueObjectiveForCompletion = createAction("OBJECTIVE_QUEUE_FOR_COMPLETION")<number>();

const ObjectiveActions = {
  queueObjectiveForCompletion
};

export type ObjectiveAction = ActionType<typeof ObjectiveActions>;

export interface ObjectiveState
{
  objectives: Objective[];
}

export const objectiveReducer: Reducer<ObjectiveState, ObjectiveAction> = (state: ObjectiveState = { objectives: [] }, action: ObjectiveAction) => {
  switch (action.type) {
    case getType(ObjectiveActions.queueObjectiveForCompletion):
      const newObjectives = [...state.objectives];
      newObjectives[action.payload].queuedForCompletion = true;
      return { ...state, objectives: newObjectives };
  }

  return state;
}