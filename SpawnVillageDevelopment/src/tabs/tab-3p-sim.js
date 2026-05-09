import { threePartySimAdditionData, threePartySimData, threePartySimNote } from '../data/3p-sim'

export const tab3pSim = {
  id: '3p-sim',
  label: '3 Party - Sim',
  rows: threePartySimData,
  notes: threePartySimNote,
  cpBuildingConfig: {
    beforeInsertIndex: 74,
    afterInsertIndex: 75,
  },
  additionRows: threePartySimAdditionData,
}
