import "reflect-metadata"; // required
import {ICompositionalRoot} from "@interfaces/composition_root";
import {serviceContainer} from "@models/inversify_config";
import {IocTypes} from "@models/ioc_types";

(async (): Promise<void> => {
    const compositionalRoot = serviceContainer.get<ICompositionalRoot>(IocTypes.ICompositionalRoot);
    await compositionalRoot.start();
})();