import { fourPartySimAdditionData, fourPartySimData, fourPartySimNote } from '../data/4p-sim'

export const tab4pSim = {
  id: '4p-sim',
  label: '4 Party - Sim',
  rows: fourPartySimData,
  notes: fourPartySimNote,
  cpBuildingConfig: {
    beforeInsertIndex: 76,
    afterInsertIndex: 77,
  },
  additionRows: fourPartySimAdditionData,
}
