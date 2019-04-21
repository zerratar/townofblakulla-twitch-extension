import GameState from "../../game-state";
import BlakullaService from "../../blakulla-service";
export interface PhaseProps {
    gameState: GameState;
    service: BlakullaService;
    channel: string;
}
