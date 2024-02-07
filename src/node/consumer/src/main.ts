import "reflect-metadata"; // required
import {ICompositionalRoot} from "./lib/interfaces/composition_root";
import {serviceContainer} from "./lib/models/inversify_config";
import {IocTypes} from "./lib/models/ioc_types";

(async (): Promise<void> => {
    const compositionalRoot = serviceContainer.get<ICompositionalRoot>(IocTypes.ICompositionalRoot);
    await compositionalRoot.start();
})();